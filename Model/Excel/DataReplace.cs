using MongoDB.Bson.Serialization.Attributes;

namespace SMIJobHeader.Model.Excel;

[BsonIgnoreExtraElements]
public class DataReplace
{
    [BsonElement("current_value")] public string? CurrentValue { get; set; }

    [BsonElement("replace_value")] public string? ReplaceValue { get; set; }

    [BsonElement("is_default")] public bool IsDefault { get; set; }
}