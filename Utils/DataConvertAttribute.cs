namespace SMIJobHeader.Utils;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class DataConvertAttribute : Attribute
{
    public DataConvertAttribute(string source,
        string defaultValue = null,
        bool throwExceptionIfSourceNotExist = true,
        object defaultValueWhenNull = null,
        string dateFomat = null,
        string numberFomat = null,
        object valueNotApprove = null
    )
    {
        Source = source;
        DefaultValue = defaultValue;
        ThrowExceptionIfSourceNotExist = throwExceptionIfSourceNotExist;
        DateFomat = dateFomat;
        NumberFomat = numberFomat;
        DefaultValueWhenNull = defaultValueWhenNull;
        ValueNotApprove = valueNotApprove;
    }

    public string Source { get; set; }
    public string DefaultValue { get; set; }
    public bool ThrowExceptionIfSourceNotExist { get; set; }
    public string DateFomat { get; set; }
    public string NumberFomat { get; set; }

    public object DefaultValueWhenNull { get; set; }

    public object ValueNotApprove { get; set; }
}