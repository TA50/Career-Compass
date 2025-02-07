using AutoMapper;
using CareerCompass.Api.Users.Contracts;
using CareerCompass.Application.Users;

namespace CareerCompass.Api.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<string, UserId>()
            .ConvertUsing(src => new UserId(Guid.Parse(src)));


        CreateMap<UserId, string>()
            .ConvertUsing(src => src.Value.ToString());


        CreateMap<User, UserDto>();
    }
}