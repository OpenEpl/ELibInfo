using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenEpl.ELibInfo
{
    public class ELibMemberInfo
    {
        public string Name { get; set; }
        public int DataType { get; set; }
        public string EnglishName { get; set; }
        public string Description { get; set; }

        [DefaultValue(ELibDeprecatedLevel.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Default { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
