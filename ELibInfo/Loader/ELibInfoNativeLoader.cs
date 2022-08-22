using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo.Loader
{
    public class ELibInfoNativeLoader : IELibInfoLoader
    {
        private const string DefaultBinFileName = "ExportELibInfoV1.exe";

        public string BinPath { get; }

        /// <summary>
        /// Create a native loader using the default bin path. No thrown even if the bin file is not found.
        /// </summary>
        public ELibInfoNativeLoader()
        {
            BinPath = new Func<string>[]
            {
                () => Path.Combine(Path.GetDirectoryName(typeof(ELibManifest).Assembly.Location), DefaultBinFileName),
                () => Path.Combine(Path.GetDirectoryName(typeof(ELibManifest).Assembly.Location), "..", "..", "contentFiles", "any", "any", DefaultBinFileName),
                () => Path.Combine(Path.GetDirectoryName(typeof(ELibManifest).Assembly.Location), "..", "..", "content", DefaultBinFileName),
                () => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DefaultBinFileName)
            }.Select(f =>
            {
                try
                {
                    return f();
                }
                catch (Exception)
                {
                    return null;
                }
            }).FirstOrDefault(x => File.Exists(x));
        }

        /// <summary>
        /// No thrown even if <paramref name="binPath"/> is invaild.
        /// </summary>
        /// <param name="binPath"></param>
        public ELibInfoNativeLoader(string binPath)
        {
            BinPath = binPath; // `null` is allowed here. we'll throw a exception when `Load` is called.
        }

        public ELibManifest Load(Guid guid, string fileName, Version version)
        {
            if (fileName == null)
            {
                throw new NotSupportedException($"Loading elib without file name is not supported by {nameof(ELibInfoNativeLoader)}",
                    new ArgumentNullException(nameof(fileName)));
            }
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException($"The file name \"{fileName}\" is invaild", nameof(fileName));
            }
            if (!File.Exists(BinPath))
            {
                throw new FileNotFoundException($"{DefaultBinFileName} is not found, which {nameof(ELibInfoNativeLoader)} requires");
            }
            string tempFile = null;
            string result;
            int exitCode;
            try
            {
                tempFile = Path.GetTempFileName();
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = BinPath;
                    process.StartInfo.Arguments = $"\"{fileName}\" \"{tempFile}\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                    result = File.ReadAllText(tempFile, Encoding.Unicode);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to export elib info for \"{fileName}\"", e);
            }
            finally
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception)
                {
                    // Nothing to do
                }
            }
            if (exitCode != 0 || string.IsNullOrWhiteSpace(result))
            {
                throw new Exception($"Failed to export elib info for \"{fileName}\", exit code = {exitCode}");
            }
            try
            {
                return JsonSerializer.Deserialize<ELibManifest>(result, JsonUtils.Options);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to parse exported elib info for \"{fileName}\"", e);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ELibInfoNativeLoader)}({nameof(BinPath)}=\"{BinPath}\")";
        }
    }
}
