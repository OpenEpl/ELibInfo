using System;

namespace OpenEpl.ELibInfo
{
    [Flags]
    public enum ELibReceiveFlags
    {
        None = 0,
        NonArrayVar = 1 << 2,
        ArrayVar = 1 << 3,
        AllVar = 1 << 4,
        Array = 1 << 5,
        All = 1 << 6,
        NonArrayVarOrVal = 1 << 9
    }
}
