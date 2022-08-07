using System;

namespace OpenEpl.ELibInfo.Loader
{
    public class ELibInfoLoader
    {
        private static Lazy<IELibInfoLoader> _default = new Lazy<IELibInfoLoader>(() =>
        {
            return new ELibInfoCacheableLoader(new ELibInfoNativeLoader());
        });
        public static IELibInfoLoader Default { get; } = _default.Value;
    }
}
