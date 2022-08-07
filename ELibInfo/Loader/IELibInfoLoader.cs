using System;

namespace OpenEpl.ELibInfo.Loader
{
    public interface IELibInfoLoader
    {
        /// <summary>
        /// Load and get information from an elib.
        /// </summary>
        /// <remarks>
        /// By design, either <paramref name="guid"/> or <paramref name="fileName"/> can be <see langword="default"/>,
        /// which means no condition is required on the corresponding field. 
        /// However, some implementations may not support that.
        /// <br />
        /// Some implementations can execute the elib, which may lead to safety problems.
        /// </remarks>
        /// <param name="guid"></param>
        /// <param name="fileName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        ELibManifest Load(Guid guid, string fileName, Version version);
    }
}
