using AutoMapper;
using API.Models;
using API.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User to RegisterDto
        CreateMap<User, RegisterDto>()
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => src.Civility.ToString()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Enum.Parse(typeof(User.UserGender), src.Gender)))
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => Enum.Parse(typeof(User.CivilStatus), src.Civility)));
    }
}
