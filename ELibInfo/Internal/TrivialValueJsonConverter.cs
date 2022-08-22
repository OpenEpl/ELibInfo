using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OpenEpl.ELibInfo.Internal
{
    internal class TrivialValueJsonConverter : JsonConverter<object>
    {
        public override bool HandleNull => true;

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var element = JsonElement.ParseValue(ref reader);
            return Translate(element);
        }

        private object Translate(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Object:
                    var dictionary = new Dictionary<string, object>();
                    foreach (var item in element.EnumerateObject())
                    {
                        dictionary.Add(item.Name, Translate(item.Value));
                    }
                    return dictionary;
                case JsonValueKind.Array:
                    var list = new List<object>(element.GetArrayLength());
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(Translate(item));
                    }
                    return list;
                default:
                    throw new Exception($"Unknown {nameof(element.ValueKind)}");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<object>(writer, value, options);
        }
    }
}
