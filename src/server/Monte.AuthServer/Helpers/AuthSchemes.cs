using Microsoft.AspNetCore.Identity;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

namespace Monte.AuthServer.Helpers;

public static class AuthSchemes
{
    public const string TokenServer = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
    public static readonly string Cookie = IdentityConstants.ApplicationScheme;
    public const string TokenValidation = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
}
