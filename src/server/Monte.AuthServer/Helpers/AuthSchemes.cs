using Microsoft.AspNetCore.Identity;
using OpenIddict.Server.AspNetCore;

namespace Monte.AuthServer.Helpers;

public static class AuthSchemes
{
    public const string Token = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
    public static readonly string Cookie = IdentityConstants.ApplicationScheme;
}