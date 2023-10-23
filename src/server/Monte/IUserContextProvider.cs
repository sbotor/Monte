namespace Monte;

public interface IUserContextProvider
{
    ValueTask<UserContext> GetContext(CancellationToken cancellationToken = default);
}

public class UserContext
{
    public string Id { get; }
    
    public UserContext(string id)
    {
        Id = id;
    }
}
