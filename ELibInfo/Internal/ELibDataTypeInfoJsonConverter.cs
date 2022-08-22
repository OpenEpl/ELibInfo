using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OpenEpl.ELibInfo.Internal
{
    internal class ELibDataTypeInfoJsonConverter : JsonConverter<ELibDataTypeInfo>
    {
        public override ELibDataTypeInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var element = JsonElement.ParseValue(ref reader);
            var kind = ELibDataTypeKind.Object;
            if (element.TryGetProperty("Kind", out var jsonElement))
            {
                if (!Enum.TryParse(jsonElement.GetString(), out kind))
                {
                    throw new Exception($"Unsupported {nameof(ELibDataTypeInfo)}.{nameof(ELibDataTypeInfo.Kind)} = {jsonElement}");
                }
            }
            switch (kind)
            {
                case ELibDataTypeKind.Object:
                    return element.Deserialize<ELibObjectInfo>(options);
                case ELibDataTypeKind.Enum:
                    return element.Deserialize<ELibEnumInfo>(options);
                case ELibDataTypeKind.Component:
                    return element.Deserialize<ELibComponentInfo>(options);
                default:
                    throw new Exception($"Unsupported {nameof(ELibDataTypeInfo)}.{nameof(ELibDataTypeInfo.Kind)} = {kind}");
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            ELibDataTypeInfo value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case ELibObjectInfo objectInfo:
                    JsonSerializer.Serialize(writer, objectInfo, options);
                    break;
                case ELibEnumInfo enumInfo:
                    JsonSerializer.Serialize(writer, enumInfo, options);
                    break;
                case ELibComponentInfo componentInfo:
                    JsonSerializer.Serialize(writer, componentInfo, options);
                    break;
            }
        }
    }
}
