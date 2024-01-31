using Microsoft.AspNetCore.Identity;

namespace Monte.AuthServer.Data;

public class AppUser : IdentityUser
{
    public bool IsExternal { get; set; }
    
    public IReadOnlyCollection<AppUserRole> UserRoles { get; set; } = null!;
}

public class AppUserRole : IdentityUserRole<string>
{
    public AppRole Role { get; set; } = null!;
}

public class AppRole : IdentityRole
{
}
