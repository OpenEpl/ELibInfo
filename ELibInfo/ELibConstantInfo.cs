using Newtonsoft.Json;

namespace OpenEpl.ELibInfo
{
    public class ELibConstantInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }
        public int DataType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
