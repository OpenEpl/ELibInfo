using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibCategoryInfo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ImageId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
