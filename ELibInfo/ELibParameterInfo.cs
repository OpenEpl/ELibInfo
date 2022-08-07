using System.ComponentModel;
using Newtonsoft.Json;

namespace OpenEpl.ELibInfo
{
    public class ELibParameterInfo
    {
        public string Name { get; set; }
        public int DataType { get; set; }
        public string Description { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Optional { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool VarArgs { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Default { get; set; }

        [DefaultValue(ELibReceiveFlags.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ELibReceiveFlags ReceiveFlags { get; set; } = ELibReceiveFlags.None;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
