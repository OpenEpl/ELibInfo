using System;
using OpenEpl.ELibInfo.Loader;
namespace DebugELibInfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var foo = new ELibInfoNativeLoader().Load(Guid.Empty, "krnln", null);
            DebugHelper.DebugUtil.ShowDebug(foo);
        }
    }
}
