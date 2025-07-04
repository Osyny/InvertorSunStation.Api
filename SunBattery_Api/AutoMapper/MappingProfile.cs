
using AutoMapper;
using SunBattery.Core.Entities;
using SunBattery_Api.Models.Dtos;
using SunBattery_Api.Models.Dtos.ProtocolData;

namespace Station.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ProtocolData, ProtocolDataDto>().ReverseMap();

           
        }
    }
}
