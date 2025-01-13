using SMIJobHeader.Constants;
using SMIJobHeader.Model.Excel;

namespace SMIJobHeader.Model.Config;

public class PropertyConfig
{
    public int Index { get; set; }
    public string? PropertyName { get; set; }
    public string? DisplayName { get; set; }
    public string? DataType { get; set; }
    public double? Width { get; set; }
    public int? Format { get; set; }
    public string? Formula { get; set; }
    public string? XpathTag { get; set; }
    public bool IsHtml { get; set; }
    public string? RegexPattern { get; set; }
    public int RegexIndexGroupCollection { get; set; }
    public Enums.ExportCellStyles BodyCellStyle { get; set; }
    public Enums.ExportCellStyles FooterCellStyle { get; set; }
    public string? FieldName { get; set; }
    public string? FieldNameMapping { get; set; }
    public string? Lable { get; set; }
    public string? FormulaExcel { get; set; }
    public bool? Required { get; set; }
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

    public int? CommentHight { get; set; }
    public bool? IsExport { get; set; }

    /// <summary>
    ///     Độ rộng của dialog comment
    /// </summary>

    public List<Merge> HeaderMerges { get; set; } = new();

    public int RowIndex { get; set; }
    public bool Active { get; set; }
}