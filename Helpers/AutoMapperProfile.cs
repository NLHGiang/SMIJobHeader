using System.Globalization;
using AutoMapper;
using SMIJobHeader.Constants;
using SMIJobHeader.Entities;
using SMIJobHeader.Model;
using SMIJobHeader.Model.Config;
using SMIJobHeader.Model.Excel;
using SMIJobHeader.Model.Message;

namespace SMIJobHeader.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MessageLogDto, MessageLog>();
        CreateMap<PropertyConfig, ExcelColumnConfig>();
        CreateMap<EInvoiceDto, EinvoiceHeader>()
            .ForMember(dest => dest.tdlap, opt => opt.MapFrom(src => ParseDate(src.nlap)))
            .ForMember(dest => dest.tthai, opt => opt.MapFrom(src => GetEInvoiceStatusCode(src.tthoadon)))
            .ForMember(dest => dest.ttxly, opt => opt.MapFrom(src => GetEInvoiceResultCode(src.kqkthdon)));

        CreateMap<EinvoiceHeader, invoiceheaders>();
    }

    private static string? ParseDate(string dateString)
    {
        var format = "dd/MM/yyyy";
        var provider = CultureInfo.InvariantCulture;

        DateTime parsedDate;
        if (DateTime.TryParseExact(dateString, format, provider, DateTimeStyles.None, out parsedDate))
            return parsedDate.AddHours(-7).ToString("yyyy-MM-ddTHH:mm:ssZ");

        return null;
    }

    private static int? GetEInvoiceStatusCode(string statusText)
    {
        return EInvoiceCrawlConstants.EInvoiceStatusMapping.TryGetValue(statusText, out var statusCode)
            ? statusCode
            : null;
    }

    private static int? GetEInvoiceResultCode(string resultText)
    {
        return EInvoiceCrawlConstants.EInvoiceResultMapping.TryGetValue(resultText, out var statusCode)
            ? statusCode
            : null;
    }
}