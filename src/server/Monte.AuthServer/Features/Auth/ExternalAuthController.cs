using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Extensions;
using Monte.AuthServer.Models;
using Monte.AuthServer.Services;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace Monte.AuthServer.Features.Auth;

[ApiController]
[Route("challenge")]
[AllowAnonymous]
public class ExternalAuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AuthSettings _settings;

    public ExternalAuthController(IUserService userService, IOptions<AuthSettings> options)
    {
        _userService = userService;
        _settings = options.Value;
    }
    
    [HttpGet("google")]
    public IActionResult Google()
        => Challenge(new(),
            authenticationSchemes: Providers.Google);

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(Providers.Google);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        var email = result.Principal.FindFirstValue(Claims.Email)!;

        var created = await _userService.EnsureExternalUserCreated(email, ExternalUserType.Google);
        if (created.ErrType != ErrorType.None)
        {
            return created.ToActionResult();
        }

        var user = created.Object!;
        var identity = new ClaimsIdentity(IdentityConstants.ExternalScheme);

        identity.SetClaim(Claims.Subject, user.Id)
            .SetClaim(Claims.Role, user.Role)
            .SetClaim(Claims.Name, user.Name);

        return SignIn(new(identity),
            new AuthenticationProperties { RedirectUri = _settings.RedirectUri.ToString()});
    }
}
