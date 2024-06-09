using API.DTOs;
using Data.Models;
using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User to RegisterDto
        CreateMap<User, RegisterDto>()
            .ForMember(dest => dest.Civility,
            opt => opt.MapFrom(src => src.Civility.ToString()))
            .ForMember(dest => dest.Gender,
            opt => opt.MapFrom(src => src.Gender.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Gender,
            opt =>
                opt
                    .MapFrom(src =>
                        Enum.Parse(typeof (User.UserGender), src.Gender)))
            .ForMember(dest => dest.Civility,
            opt =>
                opt
                    .MapFrom(src =>
                        Enum.Parse(typeof (User.CivilStatus), src.Civility)));

        CreateMap<Complaint, ComplaintDto>()
            .ForMember(dest => dest.ComplaintState,
            opt => opt.MapFrom(src => src.ComplaintState.ToString()))
            .ForMember(dest => dest.ComplaintType,
            opt => opt.MapFrom(src => src.ComplaintType.ToString()));

        CreateMap<ComplaintDto, Complaint>()
            .ForMember(dest => dest.ComplaintState,
            opt =>
                opt
                    .MapFrom(src =>
                        Enum
                            .Parse(typeof (ComplaintState),
                            src.ComplaintState)))
            .ForMember(dest => dest.ComplaintType,
            opt =>
                opt
                    .MapFrom(src =>
                        Enum.Parse(typeof (ComplaintType), src.ComplaintType)));
    }
}
