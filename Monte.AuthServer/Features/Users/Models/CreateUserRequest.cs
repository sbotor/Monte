namespace Monte.AuthServer.Features.Users.Models;

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}