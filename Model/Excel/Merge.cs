using MongoDB.Bson.Serialization.Attributes;

namespace SMIJobHeader.Model.Excel;

[BsonIgnoreExtraElements]
public class Merge
{
    [BsonElement("column_name")] public string ColumnName { get; set; }

    [BsonElement("column_range")] public string ColumnRange { get; set; }
}