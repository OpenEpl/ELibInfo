using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibConstantInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }
        public int DataType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(TrivialValueJsonConverter))]
        public object Value { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
