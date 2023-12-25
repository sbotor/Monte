namespace Monte.AuthServer.Features.Users.Models;

public class ChangeUsernameRequest
{
    public string? UserId { get; set; } = null!;
    public string NewUsername { get; set; } = null!;
}
