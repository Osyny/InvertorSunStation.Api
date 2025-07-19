
using AutoMapper;
using SunBattery.Core.Entities;
using SunBattery_Api.Models.Dtos;
using SunBattery_Api.Models.Dtos.ProtocolData;
using SunBattery_Api.Models.Dtos.Users;

namespace Station.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<ProtocolData, ProtocolDataDto>().ReverseMap();

           
        }
    }
}
