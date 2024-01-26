using Microsoft.AspNetCore.Identity;

namespace Monte.AuthServer.Data;

public class AppUser : IdentityUser
{
    public IReadOnlyCollection<AppUserRole> UserRoles { get; set; } = null!;
}

public class AppUserRole : IdentityUserRole<string>
{
    public AppRole Role { get; set; } = null!;
}

public class AppRole : IdentityRole
{
}
