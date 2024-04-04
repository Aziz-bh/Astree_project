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

        }
    }
}
