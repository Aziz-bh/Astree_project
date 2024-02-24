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
        CreateMap<User, UserVM>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.DateTime)) // Convert DateTimeOffset to DateTime
            .ReverseMap()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateTimeOffset(src.BirthDate.GetValueOrDefault())));
    
        }
    }
}
