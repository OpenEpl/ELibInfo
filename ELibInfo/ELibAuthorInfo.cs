using System.Text.Json;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    public class ELibAuthorInfo
    {
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string HomePage { get; set; }
        public string Additional { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
}
