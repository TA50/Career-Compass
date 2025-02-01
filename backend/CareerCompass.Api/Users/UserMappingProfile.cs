using AutoMapper;
using CareerCompass.Application.Users;

namespace CareerCompass.Api.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Guid  < - >  UserId
        CreateMap<UserId, Guid>()
            .ConvertUsing(src => src.Value);

        CreateMap<Guid, UserId>()
            .ConvertUsing(src => new UserId(src));


        CreateMap<string, UserId>()
            .ConvertUsing(src => new UserId(Guid.Parse(src)));


        CreateMap<UserId, string>()
            .ConvertUsing(src => src.Value.ToString());
    }
}