using MongoDB.Bson.Serialization.Attributes;
using SMIJobHeader.Constants;

namespace SMIJobHeader.Model.Excel;

[BsonIgnoreExtraElements]
public class ExcelColumnConfig
{
    public string? PropertyName { get; set; }
    public string? DisplayName { get; set; }

    public string? FieldName { get; set; }
    public string? FieldNameMapping { get; set; }
    public string? Lable { get; set; }
    public string? Formula { get; set; }
    public string? FormulaExcel { get; set; }
    public Enums.ExportCellStyles BodyCellStyle { get; set; }
    public Enums.ExportCellStyles FooterCellStyle { get; set; }
    public bool? Required { get; set; }
    public string? Format { get; set; }
    public int? MinimumValue { get; set; }
    public int? MaxLength { get; set; }
    public int? MinimumLength { get; set; }
    public int? MaxValue { get; set; }

    public string? DefaultValue { get; set; }
    public List<DataReplace>? DataReplace { get; set; }
    public bool IsValidEmail { get; set; }
    public bool IsValuePhone { get; set; }
    public string? MatchMode { get; set; }
    public string? ValueReplace { get; set; }
    public string? GroupTypeCode { get; set; }
    public string? Sum { get; set; }

    /// <summary>
    ///     Độ rộng của dialog comment
    /// </summary>
    public int? CommentWith { get; set; }

    public int? Width { get; set; }
    public bool? Active { get; set; }
    public int? CommentHight { get; set; }
    public bool? IsExport { get; set; }

    /// <summary>
    ///     Độ rộng của dialog comment
    /// </summary>

    public List<Merge> HeaderMerges { get; set; } = new();
}