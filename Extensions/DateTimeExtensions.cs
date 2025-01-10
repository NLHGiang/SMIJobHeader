using SMIJobHeader.Constants;

namespace SMIJobHeader.Extensions;

public static class DateTimeExtensions
{
    public static bool IsInSEAsiaStandardTime(this DateTime dateTime)
    {
        var seAsiaStandardTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DateTimeZone.SeAsiaStardTime);
        var seAsiaStandardTime = TimeZoneInfo.ConvertTime(dateTime, seAsiaStandardTimeZone);
        return dateTime.Kind == DateTimeKind.Local && dateTime.Equals(seAsiaStandardTime);
    }

    public static string? FormatDate(this DateTime? dateValue, string format = "")
    {
        if (!dateValue.HasValue)
        {
            WriteLogError($"[{dateValue}] Null or Empty");
            return null;
        }

        try
        {
            return dateValue.Value.ToString("yyyyMM");
        }
        catch (Exception ex)
        {
            WriteLogError($"[{dateValue}] An error occurred: {ex.Message}");
            return null;
        }
    }

    public static void WriteLogError(string messageLog)
    {
        try
        {
            using (var w = File.AppendText(
                       @$"C:\SMIJobXml-DateTime-ErrorLog\{DateTime.Now.ToString("yyyy_MM_dd")}.txt"))
            {
                w.WriteLine(messageLog);
            }
        }
        catch
        {
        }
    }
}