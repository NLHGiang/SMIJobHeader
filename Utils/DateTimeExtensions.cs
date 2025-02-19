using System.Globalization;
using SMIJobHeader.Constants;

namespace SMIJobHeader.Utils;

public static class DateTimeExtensions
{
    public static bool IsInSEAsiaStandardTime(this DateTime dateTime)
    {
        var seAsiaStandardTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DateTimeZone.SeAsiaStardTime);
        var seAsiaStandardTime = TimeZoneInfo.ConvertTime(dateTime, seAsiaStandardTimeZone);
        return dateTime.Kind == DateTimeKind.Local && dateTime.Equals(seAsiaStandardTime);
    }

    public static string? FormatDate(this string? dateValue, string format = "")
    {
        if (string.IsNullOrWhiteSpace(dateValue)) return null;

        string[] dateTimeFormats =
        {
            "M/d/yyyy hh:mm:ss tt",
            "MM/dd/yyyy hh:mm:ss tt",
            "MM/dd/yyyy HH:mm:ss tt",
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy HH:mm:ss tt",
            "MM/dd/yyyy h:mm:ss tt",
            "MM/dd/yyyy H:mm:ss tt",
            "dd/MM/yyyy h:mm:ss tt",
            "dd/MM/yyyy H:mm:ss tt",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss SA",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "dd/MM/yyyyTHH:mm:ss"
        };

        try
        {
            var parsedDate = DateTime.ParseExact(
                dateValue,
                dateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
            return parsedDate.ToString(format);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid date format.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}