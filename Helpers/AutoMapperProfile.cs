using AutoMapper;
using SMIJobXml.Entities;
using SMIJobXml.Model.Message;

namespace SMIJobXml.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MessageLogDto, MessageLog>();
        }
    }
}
