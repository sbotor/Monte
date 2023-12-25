namespace Monte.AuthServer.Features.Users.Models;

public class ChangePasswordRequest
{
    public string? UserId { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
