using API.DTOs;
using Data.Models;
using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User to RegisterDto and vice versa
        CreateMap<User, RegisterDto>()
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => src.Civility.ToString()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Enum.Parse(typeof(User.UserGender), src.Gender)))
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => Enum.Parse(typeof(User.CivilStatus), src.Civility)));

        // User to UserDto and vice versa
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => src.Civility.ToString()))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Enum.Parse(typeof(User.UserGender), src.Gender)))
            .ForMember(dest => dest.Civility, opt => opt.MapFrom(src => Enum.Parse(typeof(User.CivilStatus), src.Civility)));

        // Complaint to ComplaintDto and vice versa
        CreateMap<Complaint, ComplaintDto>()
            .ForMember(dest => dest.ComplaintState, opt => opt.MapFrom(src => src.ComplaintState.ToString()))
            .ForMember(dest => dest.ComplaintType, opt => opt.MapFrom(src => src.ComplaintType.ToString()))
            .ReverseMap()
            .ForMember(dest => dest.ComplaintState, opt => opt.MapFrom(src => Enum.Parse(typeof(ComplaintState), src.ComplaintState)))
            .ForMember(dest => dest.ComplaintType, opt => opt.MapFrom(src => Enum.Parse(typeof(ComplaintType), src.ComplaintType)));

        // UserUpdateDTO to User
        CreateMap<UserUpdateDTO, User>()
            .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordRestToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .ForMember(dest => dest.Complaints, opt => opt.Ignore())
            .ForMember(dest => dest.Contracts, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore());
    }
}
