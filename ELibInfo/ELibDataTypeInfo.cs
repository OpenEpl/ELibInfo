using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenEpl.ELibInfo.Internal;

namespace OpenEpl.ELibInfo
{
    [JsonConverter(typeof(ELibDataTypeInfoJsonConverter))]
    public abstract class ELibDataTypeInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }
        public ImmutableArray<ELibEvnetInfo> Evnets { get; set; }
        public ImmutableArray<ELibMemberInfo> Members { get; set; }
        public ImmutableArray<int> Methods { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        public abstract ELibDataTypeKind Kind { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, JsonUtils.Options);
        }
    }
    public class ELibObjectInfo : ELibDataTypeInfo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Object;
    }
    public class ELibEnumInfo : ELibDataTypeInfo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Enum;
    }
    public class ELibComponentInfo : ELibDataTypeInfo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Component;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ImageId { get; set; } = 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsContainer { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsTabControl { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsFunctional { get; set; } = false;

        public bool CanFocus { get; set; } = true;

        /// <summary>
        /// If <see cref="CanFocus"/> is <see langword="false"/>, then this property must be <see langword="true"/>.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool NoTabStopByDefault { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsMessageFilter { get; set; } = false;
    }
}
