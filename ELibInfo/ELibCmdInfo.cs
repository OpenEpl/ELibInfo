using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibCmdInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; } = -1;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool DebugOnly { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ReturnDataType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool ReturnArray { get; set; } = false;

        public ImmutableArray<ELibParameterInfo> Parameters { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ELibCmdKind Kind { get; set; } = ELibCmdKind.Normal;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ELibLearningCost LearningCost { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
