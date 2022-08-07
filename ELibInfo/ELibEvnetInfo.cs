using System.Collections.Immutable;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenEpl.ELibInfo
{
    public class ELibEvnetInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [DefaultValue(ELibDeprecatedLevel.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ReturnDataType { get; set; }

        public ImmutableArray<ELibParameterInfo> Parameters { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
