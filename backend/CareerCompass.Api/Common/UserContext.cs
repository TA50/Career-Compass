using CareerCompass.Application.Users;

namespace CareerCompass.Api.Common;

public class UserContext
{
    public UserId UserId { get; private set; }


    public void SetUserId(UserId userId)
    {
        UserId = userId;
    }
}