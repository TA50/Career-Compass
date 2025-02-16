using AutoMapper;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Users;

namespace CareerCompass.Api.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<string, UserId>()
            .ConvertUsing(src => UserId.Create(Guid.Parse(src)));


        CreateMap<UserId, string>()
            .ConvertUsing(src => src.Value.ToString());


        CreateMap<User, UserDto>();



    }
}