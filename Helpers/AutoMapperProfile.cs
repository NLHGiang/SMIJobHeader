using AutoMapper;
using SMIJobHeader.Entities;
using SMIJobHeader.Model.Message;

namespace SMIJobHeader.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MessageLogDto, MessageLog>();
    }
}