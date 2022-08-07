using System;
using System.Collections.Immutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace OpenEpl.ELibInfo
{
    public class ELibManifest
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        [JsonConverter(typeof(VersionConverter))]
        public Version Version { get; set; }
        public int VersionCode { get; set; }
        [JsonConverter(typeof(VersionConverter))]
        public Version MinRequiredEplVersion { get; set; }
        [JsonConverter(typeof(VersionConverter))]
        public Version MinRequiredKrnlnVersion { get; set; }
        public string Description { get; set; }
        public ImmutableArray<ELibDataTypeInfo> DataTypes { get; set; }
        public ImmutableArray<ELibCategoryInfo> Categories { get; set; }
        public ImmutableArray<ELibCmdInfo> Cmds { get; set; }
        public ImmutableArray<ELibConstantInfo> Constants { get; set; }
        public ELibAuthorInfo Author { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}