namespace Monte;

public interface IUserContext
{
    ValueTask<UserInfo> GetUser();
}

public record UserInfo(string Id);