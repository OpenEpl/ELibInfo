using System.Collections.Immutable;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenEpl.ELibInfo
{
    public class ELibCmdInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }

        [DefaultValue(-1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int CategoryId { get; set; } = -1;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool DebugOnly { get; set; } = false;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ReturnDataType { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ReturnArray { get; set; } = false;

        public ImmutableArray<ELibParameterInfo> Parameters { get; set; }

        [DefaultValue(ELibDeprecatedLevel.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [DefaultValue(ELibCmdKind.Normal)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELibCmdKind Kind { get; set; } = ELibCmdKind.Normal;

        [JsonConverter(typeof(StringEnumConverter))]
        public ELibLearningCost LearningCost { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
