using System.Collections.Immutable;
using System.ComponentModel;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenEpl.ELibInfo
{
    [JsonConverter(typeof(JsonSubtypes), "Kind")]
    [JsonSubtypes.KnownSubType(typeof(ELibObjectInfo), ELibDataTypeKind.Object)]
    [JsonSubtypes.KnownSubType(typeof(ELibEnumInfo), ELibDataTypeKind.Enum)]
    [JsonSubtypes.KnownSubType(typeof(ELibComponentInfo), ELibDataTypeKind.Component)]
    [JsonSubtypes.FallBackSubType(typeof(ELibObjectInfo))]
    public abstract class ELibDataTypeInfo
    {
        public string Name { get; set; }
        public string EnglshName { get; set; }
        public string Description { get; set; }
        public ImmutableArray<ELibEvnetInfo> Evnets { get; set; }
        public ImmutableArray<ELibMemberInfo> Members { get; set; }
        public ImmutableArray<int> Methods { get; set; }

        [DefaultValue(ELibDeprecatedLevel.None)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ELibDeprecatedLevel Deprecated { get; set; } = ELibDeprecatedLevel.None;

        [DefaultValue(ELibDataTypeKind.Object)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract ELibDataTypeKind Kind { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class ELibObjectInfo : ELibDataTypeInfo
    {
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Object;
    }
    public class ELibEnumInfo : ELibDataTypeInfo
    {
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Enum;
    }
    public class ELibComponentInfo : ELibDataTypeInfo
    {
        public override ELibDataTypeKind Kind => ELibDataTypeKind.Component;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ImageId { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsContainer { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsTabControl { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsFunctional { get; set; } = false;

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool CanFocus { get; set; } = true;

        /// <summary>
        /// If <see cref="CanFocus"/> is <see langword="false"/>, then this property must be <see langword="true"/>.
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool NoTabStopByDefault { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsMessageFilter { get; set; } = false;
    }
}
