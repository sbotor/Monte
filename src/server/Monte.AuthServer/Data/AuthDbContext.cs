using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Monte.AuthServer.Data;

public class AuthDbContext
    : IdentityDbContext<
        AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(x =>
        {
            x.HasMany(y => y.UserRoles)
                .WithOne()
                .HasForeignKey(y => y.UserId);
        });

        builder.Entity<AppRole>(x =>
        {
            x.HasMany<AppUserRole>()
                .WithOne(x => x.Role)
                .HasForeignKey(y => y.RoleId);
        });
    }
}

internal class AuthDbContextDesignFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql("Host=127.0.0.1:5432;Database=Monte;Username=postgres;Password=postgres;")
            .UseOpenIddict()
            .Options;

        return new(options);
    }
}
