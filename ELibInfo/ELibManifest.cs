using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibManifest
    {
        [JsonConverter(typeof(AdaptiveGuidJsonConverter))]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public Version Version { get; set; }
        public int VersionCode { get; set; }
        public Version MinRequiredEplVersion { get; set; }
        public Version MinRequiredKrnlnVersion { get; set; }
        public string Description { get; set; }
        public ImmutableArray<ELibDataTypeInfo> DataTypes { get; set; }
        public ImmutableArray<ELibCategoryInfo> Categories { get; set; }
        public ImmutableArray<ELibCmdInfo> Cmds { get; set; }
        public ImmutableArray<ELibConstantInfo> Constants { get; set; }
        public ELibAuthorInfo Author { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
