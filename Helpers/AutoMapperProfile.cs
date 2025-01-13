using AutoMapper;
using SMIJobHeader.Entities;
using SMIJobHeader.Model;
using SMIJobHeader.Model.Config;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Model.Message;
using System.Globalization;

namespace SMIJobHeader.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MessageLogDto, MessageLog>();
        CreateMap<PropertyConfig, ExcelColumnConfig>();
        CreateMap<EInvoiceDto, EinvoiceHeader>()
            .ForMember(dest => dest.tdlap, opt => opt.MapFrom(src => ParseDate(src.nlap)));
    }

    public static string? ParseDate(string dateString)
    {
        var format = "dd/MM/yyyy";
        var provider = CultureInfo.InvariantCulture;

        DateTime parsedDate;
        if (DateTime.TryParseExact(dateString, format, provider, DateTimeStyles.None, out parsedDate))
            return parsedDate.AddHours(-7).ToString("yyyy-MM-ddTHH:mm:ssZ");

        return null;
    }
}