using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo.Loader
{
    public class ELibInfoCacheableLoader : IELibInfoLoader
    {
        /// <summary>
        /// Specify a cache path, `null` is not allowed here.
        /// </summary>
        public string CachePath { get; }

        /// <summary>
        /// Specify a source loader, which is used when no available cache is found. `null` is allowed here.
        /// </summary>
        public IELibInfoLoader Source { get; }

        /// <summary>
        /// An elib info loader with file-based cache.
        /// </summary>
        /// <param name="cachePath">Specify a cache path, `null` is not allowed here.</param>
        /// <param name="source">Specify a source loader, which is used when no available cache is found. `null` is allowed here.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ELibInfoCacheableLoader(string cachePath, IELibInfoLoader source)
        {
            CachePath = cachePath ?? throw new ArgumentNullException(nameof(cachePath));
            Source = source;
        }

        public ELibInfoCacheableLoader(IELibInfoLoader source)
        {
            string cachePath;
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    cachePath = Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData,
                        Environment.SpecialFolderOption.Create);
                    cachePath = Path.Combine(cachePath, $"{nameof(OpenEpl)}.{nameof(ELibInfo)}", "Cache");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    cachePath = Environment.GetEnvironmentVariable("XDG_CACHE_HOME")
                        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
                    cachePath = Path.Combine(cachePath, $"{nameof(OpenEpl)}.{nameof(ELibInfo)}");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Caches");
                    cachePath = Path.Combine(cachePath, $"{nameof(OpenEpl)}.{nameof(ELibInfo)}");
                }
                else
                {
                    throw new PlatformNotSupportedException("Cannot get default cache path for this platform.");
                }
            }
            catch (Exception)
            {
                cachePath = Path.GetTempPath(); //fallback
            }
            CachePath = cachePath;
            Source = source;
        }

        private const string FileExt = ".v1.json";
        private void SerializeToFile(string path, ELibManifest libInfo)
        {
            using (var stream = File.Open(path, FileMode.Create))
            {
                JsonSerializer.Serialize(stream, libInfo, JsonUtils.Options);
            }
        }
        private ELibManifest DeserializeFromFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return JsonSerializer.Deserialize<ELibManifest>(stream, JsonUtils.Options);
            }
        }
        private ELibManifest LoadFromSource(Guid guid, string fileName, Version version)
        {
            var libInfo = Source.Load(guid, fileName, version);
            try
            {
                var libCachePath = Path.Combine(CachePath, $"{libInfo.Guid:N}_{libInfo.FileName}");
                Directory.CreateDirectory(libCachePath);
                SerializeToFile(Path.Combine(libCachePath, $"{libInfo.Version}{FileExt}"), libInfo);
            }
            catch (Exception)
            {
                // Nothing to do
            }
            return libInfo;
        }
        public ELibManifest Load(Guid guid, string fileName, Version version)
        {
            (string path, Version version) best = (null, null);
            try
            {
                var searchPattern = (guid == Guid.Empty ? "*" : guid.ToString("N")) + "_" + (string.IsNullOrEmpty(fileName) ? "*" : fileName);
                var libCachePath = Directory.GetDirectories(CachePath, searchPattern).FirstOrDefault();
                if (libCachePath != null)
                {
                    if (version != null)
                    {
                        var bestPath = Path.Combine(libCachePath, $"{version}{FileExt}");
                        if (File.Exists(bestPath))
                        {
                            best = (path: bestPath, version);
                        }
                    }
                    if (best.path == null)
                    {
                        var allPossible = Directory.GetFiles(libCachePath, $"*{FileExt}");
                        best = allPossible.Select(x =>
                        {
                            if (Version.TryParse(Path.GetFileName(x).Substring(0, x.Length - FileExt.Length), out var v))
                            {
                                return (path: x, version: v);
                            }
                            else
                            {
                                return (path: x, version: null);
                            }
                        }).Where(x => x.version != null).Aggregate((x, y) => x.version.CompareTo(y.version) > 0 ? x : y);
                    }
                }
            }
            catch (Exception e)
            {
                if (Source == null)
                {
                    throw new Exception($"Failed to find cache for elib \"{fileName}\"", e);
                }
            }
            if (best.path == null || best.version == null)
            {
                if (Source == null)
                {
                    throw new Exception($"Failed to find cache for elib \"{fileName}\"");
                }
                return LoadFromSource(guid, fileName, version);
            }
            if (Source == null)
            {
                return DeserializeFromFile(best.path);
            }
            if (version != null && best.version >= version)
            {
                try
                {
                    return DeserializeFromFile(best.path);
                }
                catch (Exception)
                {
                }
                return LoadFromSource(guid, fileName, version);
            }
            else
            {
                ELibManifest fromSource = null;
                try
                {
                    fromSource = LoadFromSource(guid, fileName, version);
                }
                catch (Exception)
                {
                }
                if (fromSource == null)
                {
                    return DeserializeFromFile(best.path);
                }
                if (best.version > fromSource.Version)
                {
                    try
                    {
                        return DeserializeFromFile(best.path);
                    }
                    catch (Exception)
                    {
                    }
                }
                return fromSource;
            }
        }
    }
}
