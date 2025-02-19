using System.Reflection;
using SMIJobHeader.Constants;
using SMIJobHeader.Extensions;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Utils;
using TypeSupport.Extensions;

namespace SMIJobHeader.Common.Excel;

public static class DataValidation
{
    public static MList<MValidationResult> IsValid<T>(this T t, int record,
        Dictionary<string, ExcelColumnConfig> dicProperty)
    {
        List<ExcelColumnConfig> objectProperties = dicProperty.Select(x => x.Value).ToList();
        return t.IsValid(record, objectProperties, true);
    }

    public static MList<MValidationResult> IsValid<T>(this T t, int record, List<ExcelColumnConfig> objectProperties,
        bool showLableExcel = false)
    {
        MList<MValidationResult> result = new();
        foreach (var item in objectProperties)
        {
            if (item.FieldName.IsNullOrEmpty() || item.FormulaExcel.IsNotNullOrEmpty() ||
                item.Formula.IsNotNullOrEmpty()) continue;

            var propertyInfo = GetProperty(t, item.FieldName);
            if (propertyInfo == null) continue;
            var value = propertyInfo.GetValue(t, null);
            result.AddRange(GetError(item, value, record, showLableExcel));
        }

        return result;
    }

    private static List<MValidationResult> GetError(ExcelColumnConfig columnConfig, object value, int record,
        bool showLableExcel = false)
    {
        MList<MValidationResult> result = new();
        var fileName = showLableExcel ? columnConfig.Lable : columnConfig.FieldName;
        if (columnConfig.Required.HasValue && columnConfig.Required.Value && IsRequired(value))
        {
            var messgae = string.Format(ErrorMessageValid.MsgRequired, fileName, record);
            result.Add(new MValidationResult(record, messgae, string.Empty));
        }

        if (columnConfig.MinimumLength.HasValue && IsMinimumLength(value, columnConfig.MinimumLength.Value))
        {
            var messgae = string.Format(ErrorMessageValid.MsgMinimumLength, fileName, columnConfig.MinimumLength.Value,
                record);
            result.Add(new MValidationResult(record, messgae, string.Empty));
        }

        if (columnConfig.MaxLength.HasValue && IsOverMaxLenght(value, columnConfig.MaxLength.Value))
        {
            var messgae = string.Format(ErrorMessageValid.MsgMaxLength, fileName, columnConfig.MaxLength.Value, record);
            result.Add(new MValidationResult(record, messgae, string.Empty));
        }

        if (columnConfig.MinimumValue.HasValue && IsMinimumValue(value, columnConfig.MinimumValue.Value))
        {
            var messgae = string.Format(ErrorMessageValid.MsgMinimumValue, fileName, columnConfig.MinimumValue.Value,
                record);
            result.Add(new MValidationResult(record, messgae, string.Empty));
        }

        if (columnConfig.MaxValue.HasValue && IsOverValue(value, columnConfig.MaxValue.Value))
        {
            var messgae = string.Format(ErrorMessageValid.MsgMaxLength, fileName, columnConfig.MaxValue.Value, record);
            result.Add(new MValidationResult(record, messgae, string.Empty));
        }

        return result;
    }

    private static bool IsRequired(object value)
    {
        if (value == null) return true;

        return false;
    }

    private static bool IsOverMaxLenght(object value, int maxLength)
    {
        if (value == null) return false;
        var type = value.GetType().Name.ToLower();
        return DATATYPE.TypeCheckMinMaxLenght.Contains(type) && value.ToString().Length > maxLength;
    }

    private static bool IsMinimumLength(object value, int minLength)
    {
        if (value == null) return true;
        var type = value.GetType().Name.ToLower();
        return DATATYPE.TypeCheckMinMaxLenght.Contains(type) && value.ToString().Length < minLength;
    }

    private static bool IsOverValue(object value, int maxLength)
    {
        if (value == null) return false;
        var type = value.GetType().Name.ToLower();
        return DATATYPE.TypeCheckMinMaxValue.Contains(type) && value.ToString().Length > maxLength;
    }

    private static bool IsMinimumValue(object value, int minValue)
    {
        if (value == null) return true;
        var type = value.GetType().Name.ToLower();
        return DATATYPE.TypeCheckMinMaxValue.Contains(type) && value.ToString().Length < minValue;
    }

    public static PropertyInfo GetProperty<T>(this T t, string fieldName)
    {
        PropertyInfo prop = null;
        if (fieldName.IsNullOrEmpty()) return prop;
        var type = t.GetExtendedType();
        foreach (var item in type.Properties)
            if (item.Name.IsMeanEquals(fieldName))
            {
                prop = item;
                break;
            }

        return prop;
    }
}