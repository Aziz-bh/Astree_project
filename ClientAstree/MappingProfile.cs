using AutoMapper;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
                       CreateMap<UserDto, UserVM>().ReverseMap();
        CreateMap<UserDto, UserVM>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.DateTime)) // Assuming src.BirthDate is DateTimeOffset
            .ReverseMap()
            ;

            // New mappings for Automobile and Property
            CreateMap<AutomobileDto, AutomobileVM>()
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType.ToString()))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType.ToString()))
                .ForMember(dest => dest.Guarantees, opt => opt.MapFrom(src => src.Guarantees.ToString()));

            CreateMap<PropertyDto, PropertyVM>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType.ToString()))
                .ForMember(dest => dest.Coverage, opt => opt.MapFrom(src => src.Coverage.ToString()));
      
        }
    }
}
