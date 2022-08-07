using Newtonsoft.Json;
namespace OpenEpl.ELibInfo
{
    public class ELibCategoryInfo
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ImageId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
