using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibEventInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ReturnDataType { get; set; }

        public ImmutableArray<ELibParameterInfo> Parameters { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
