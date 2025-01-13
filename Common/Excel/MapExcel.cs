using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SMIJobHeader.Common.Excel;
using SMIJobHeader.Constants;
using SMIJobHeader.Extensions;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Utils;
using System.Globalization;
using System.Text;

namespace SMIJobHeader.Common.Excel;

public static class MapExcel
{
    public static async Task<List<T>> ReadExcel<T>(Stream stream, Dictionary<string, ExcelColumnConfig> configColumn,
        int skipRow = 0, int headerRowIndex = 1)
    {
        MList<MValidationResult> errorMapExcels = new();
        List<T> uploadExcelDto = Convert<T>(stream,
            ExcelSheet.SheetDefault, configColumn, ref errorMapExcels, skipRow, headerRowIndex);
        if (errorMapExcels.Count > 0)
        {
            var validationResults = string.Join(",", errorMapExcels.Select(p => p.ToString()).ToList());
            throw new BusinessLogicException(ResultCode.DataInvalid, validationResults);
        }

        return uploadExcelDto;
    }

    public static List<T> Convert<T>(Stream stream, string sheetName,
        Dictionary<string, ExcelColumnConfig> objectProperty, ref MList<MValidationResult> errorMapExcels,
        int skipRow = 0, int headerRowIndex = 1)
    {
        List<T> result = new();
        var openSetting = GetOpenSetting();

        using (var doc = SpreadsheetDocument.Open(stream, true, openSetting))
        {
            var wbPart = doc.WorkbookPart;
            var worksheet = GetWorksheetPart(wbPart, sheetName).Worksheet;
            IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

            var selectedRows = rows.Skip(skipRow);

            Dictionary<string, ExcelColumnConfig> headerMapping =
                MappingColumn(objectProperty, selectedRows, doc, headerRowIndex);
            if (!headerMapping.Any())
            {
                errorMapExcels.Add(new MValidationResult(0, "Dữ liệu file excel chưa được thay đổi", "ExcelTemplate"));
                return result;
            }

            result = CreateMapColumn<T>(selectedRows, headerMapping, doc, ref errorMapExcels);
        }

        return result;
    }

    private static WorksheetPart GetWorksheetPart(WorkbookPart workbookPart, string sheetName)
    {
        var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => sheetName.Equals(s.Name));
        if (sheet == null || sheet.Id == null) sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
        if (sheet == null || sheet.Id == null) throw new Exception("Sheet not contain in workbook");
        return (WorksheetPart)workbookPart.GetPartById(sheet.Id);
    }

    private static Dictionary<int, ExcelColumnConfig> MappingIndex(Dictionary<string, ExcelColumnConfig> objectProperty,
        IEnumerable<Row> rows, SpreadsheetDocument doc)
    {
        var keyValuePairs = new Dictionary<int, ExcelColumnConfig>();
        var rowHeader = rows.FirstOrDefault(p => p.RowIndex != null && p.RowIndex.Value == 1);
        if (rowHeader == null) throw new Exception("Sheet not contain in header column");
        var index = 0;
        foreach (var cell in rowHeader.Descendants<Cell>())
        {
            var colunmName = GetCellValue(doc, cell);
            if (!objectProperty.ContainsKey(colunmName))
            {
                index++;
                continue;
            }

            keyValuePairs.Add(index, objectProperty[colunmName]);
            index++;
        }

        return keyValuePairs;
    }

    private static Dictionary<string, ExcelColumnConfig> MappingColumn(
        Dictionary<string, ExcelColumnConfig> objectProperty, IEnumerable<Row> rows, SpreadsheetDocument doc,
        int headerRowIndex = 1)
    {
        var keyValuePairs = new Dictionary<string, ExcelColumnConfig>();
        var rowHeader = rows.FirstOrDefault(p => p.RowIndex != null && p.RowIndex.Value == headerRowIndex);
        if (rowHeader == null) throw new Exception("Sheet not contain in header column");
        foreach (var cell in rowHeader.Descendants<Cell>())
        {
            var colunmName = GetCellValue(doc, cell);
            if (colunmName.IsNullOrEmpty() || !objectProperty.ContainsKey(colunmName)) continue;
            var column = cell.CellReference?.Value.RemoveNumber();
            if (column.IsNullOrEmpty() || keyValuePairs.ContainsKey(column)) continue;
            keyValuePairs.Add(column, objectProperty[colunmName]);
        }

        return keyValuePairs;
    }

    private static string GetCellValue(SpreadsheetDocument doc, Cell cell)
    {
        var value = cell.CellValue?.InnerText ?? cell.InnerText;
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value))
                .InnerText;
        return value;
    }

    private static void SetDataToObject(SpreadsheetDocument doc, Row row,
        Dictionary<string, ExcelColumnConfig> headerConfig,
        object obj,
        int record, ref MList<MValidationResult> errorMapExcels)
    {
        foreach (var cell in row.Descendants<Cell>())
        {
            var cellValue = GetCellValue(doc, cell);
            var excelColumnName = cell.CellReference?.Value.RemoveNumber();
            var column = cell.CellReference?.Value.RemoveNumber();
            if (column.IsNullOrEmpty() || !headerConfig.ContainsKey(excelColumnName)) continue;
            var propertyConfig = headerConfig[excelColumnName];
            SetPropertyValue(obj, record, propertyConfig, cellValue, ref errorMapExcels);
        }

        errorMapExcels.AddRange(obj.IsValid(record, headerConfig));
    }

    private static string GetColumnNameExcel(string fileName, Dictionary<string, ExcelColumnConfig> headerMapping)
    {
        var columnName = "A";
        if (headerMapping == null || !headerMapping.Any()) return columnName;
        var kvp = headerMapping.FirstOrDefault(p => fileName.IsMeanEquals(p.Value.FieldName));
        if (!kvp.Equals(default(KeyValuePair<string, ExcelColumnConfig>))) columnName = kvp.Key;
        return columnName;
    }

    private static List<T> CreateMapColumn<T>(IEnumerable<Row> rows,
        Dictionary<string, ExcelColumnConfig> headerMapping, SpreadsheetDocument doc,
        ref MList<MValidationResult> errorMapExcels)
    {
        var lsObj = new List<T>();
        var datas = rows.Where(p => p.RowIndex.Value != 1);
        var record = 1;

        foreach (var item in datas)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            foreach (var cell in item.Descendants<Cell>())
            {
                var cellValue = GetCellValue(doc, cell);
                var excelColumnName = cell.CellReference?.Value.RemoveNumber();
                if (!headerMapping.ContainsKey(excelColumnName)) continue;
                var propertyConfig = headerMapping[excelColumnName];
                SetPropertyValue(obj, record, propertyConfig, cellValue, ref errorMapExcels);
            }

            errorMapExcels.AddRange(obj.IsValid(record, headerMapping));
            lsObj.Add(obj);
            record++;
        }

        return lsObj;
    }

    private static string GetCellValue(SpreadsheetDocument doc, Row row, string columnName)
    {
        var cellValue = string.Empty;
        var cell = row.Descendants<Cell>().FirstOrDefault(c => columnName.IsMeanEquals(c.CellReference?.Value));
        if (cell == null) return cellValue;
        cellValue = GetCellValue(doc, cell);
        return cellValue;
    }

    private static bool IsRowBlank(SpreadsheetDocument doc, Row row)
    {
        StringBuilder cellValue = new();
        var cells = row.Descendants<Cell>();
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            var textOfCell = GetCellValue(doc, cell);
            cellValue.Append(textOfCell);
        }

        if (cellValue.Length == 0) return true;
        var valueOfRow = cellValue.ToString().Replace(",", "").Replace(";", "").Replace(".", "");
        return valueOfRow.IsNullOrEmpty();
    }

    private static void SetPropertyValue<T>(T t, int record, ExcelColumnConfig property, object value,
        ref MList<MValidationResult> errorMapExcels)
    {
        var prop = t.GetProperty(property.FieldName);
        if (prop == null)
        {
            var msg = string.Format(ErrorMessageValid.MsgPropertyNotFound, property.FieldName);
            errorMapExcels.Add(new MValidationResult(record, prop.Name, msg));
            return;
        }

        if (value == null || "".Equals(value)) return;
        var destData = ConvertData(record, prop.PropertyType, prop.Name, value, property, ref errorMapExcels);
        prop.SetValue(t, destData);
    }

    private static object ConvertData(int index, Type destDataType, string propertyName, object srcData,
        ExcelColumnConfig propertyConfig, ref MList<MValidationResult> errorMapExcels)
    {
        if (propertyConfig == null || propertyConfig.Format == null)
            return ConvertData(index, destDataType, propertyConfig.FieldName, srcData, ref errorMapExcels);

        var result = FormatByConfig(srcData, destDataType, propertyConfig);
        return ConvertData(index, destDataType, propertyConfig.FieldName, result, ref errorMapExcels);
    }

    private static object ConvertData(int record, Type destDataType, string lable, object srcData,
        ref MList<MValidationResult> errorMapExcels)
    {
        try
        {
            return ChangeTypeValue(destDataType, srcData);
        }
        catch (InvalidCastException)
        {
            var msg = string.Format(ErrorMessageValid.MsgDatatypeInvalid, lable);
            errorMapExcels.Add(new MValidationResult(record, lable, msg));
            return null;
        }
        catch (FormatException)
        {
            var msg = string.Format(ErrorMessageValid.MsgDatatypeInvalid, lable);
            errorMapExcels.Add(new MValidationResult(record, lable, msg));
            return null;
        }
    }

    private static object ChangeTypeValue(Type destDataType, object srcData)
    {
        object destData;
        if (srcData == null)
        {
            destData = srcData;
            return destData;
        }

        var propertyType = Nullable.GetUnderlyingType(destDataType) ?? destDataType;
        var dataType = propertyType.GetPropertyName(out var isNullable);
        switch (dataType)
        {
            case DATATYPE.DOUBLE:
                destData = srcData.GetValueDouble(isNullable);
                break;
            case DATATYPE.DECIMAL:
                destData = srcData.GetValueDecimal(isNullable);
                break;
            case DATATYPE.INT32:
                destData = (int)System.Convert.ChangeType(srcData, typeof(int), CultureInfo.InvariantCulture);
                break;
            case DATATYPE.BOOLEAN:
                destData = srcData.ToBoolean();
                break;
            //case DATATYPE.DATETIME:
            //    destData = DateTime.ParseExact(srcData.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    break;
            default:
                destData = System.Convert.ChangeType(srcData, propertyType);
                break;
        }

        return destData;
    }

    public static string GetPropertyName(Type type)
    {
        string result;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            result = type.GenericTypeArguments[0].FullName;
        else
            result = type.FullName;
        return result.ReplaceAndTrim("System.", "").ToLower();
    }

    private static object FormatByConfig(object srcData, Type destDataType, ExcelColumnConfig propertyConfig)
    {
        object result = null;
        if (srcData == null) return result;

        try
        {
            var propertyType = Nullable.GetUnderlyingType(destDataType) ?? destDataType;
            var dataType = propertyType.GetPropertyName(out var isNullable);
            result = dataType switch
            {
                DATATYPE.DOUBLE => srcData,
                DATATYPE.DECIMAL => srcData,
                DATATYPE.DATETIME => DateTime.ParseExact(srcData.ToString(), propertyConfig.Format,
                    CultureInfo.InvariantCulture),
                DATATYPE.BOOLEAN => srcData,
                DATATYPE.INT32 => srcData,
                _ => srcData
            };
            return result;
        }
        catch
        {
            return srcData;
        }
    }

    private static OpenSettings GetOpenSetting()
    {
        return new OpenSettings
        {
            RelationshipErrorHandlerFactory = package => { return new UriRelationshipErrorHandler(); }
        };
    }
}