using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Monte.AuthServer.Features.Auth;

[ApiController]
[Route("connect")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AuthSettings _settings;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        IOptions<AuthSettings> options)
    {
        _signInManager = signInManager;
        _settings = options.Value;
    }
    
    [HttpPost("authorize")]
    [HttpGet("authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = ExtractRequest();

        if (request.HasPrompt(Prompts.Login))
        {
            return LoginRedirect();
        }

        var result = await HttpContext.AuthenticateAsync(
            AuthSchemes.Cookie);

        if (!result.Succeeded)
        {
            return LoginRedirect();
        }

        var claims = new Claim[]
        {
            new(Claims.Subject,
                result.Principal.FindFirstValue(Claims.Subject)!),
            new(Claims.Role,
                result.Principal.FindFirstValue(Claims.Role)!),
            new(Claims.Name,
                result.Principal.FindFirstValue(Claims.Name)!)
        };

        var identity = new ClaimsIdentity(claims, AuthSchemes.TokenServer);
        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(x => x.Type switch
        {
            Claims.Name when identity.HasScope(Scopes.Profile) => new[]
            {
                Destinations.AccessToken, Destinations.IdentityToken
            },
            Claims.Role when identity.HasScope(Scopes.Roles) => new[]
            {
                Destinations.AccessToken, Destinations.IdentityToken
            },
            _ => new[] { Destinations.AccessToken }
        });
        var principal = new ClaimsPrincipal(identity);

        return SignIn(principal, AuthSchemes.TokenServer);
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = ExtractRequest();

        ClaimsPrincipal principal;
        if (request.IsAuthorizationCodeGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(AuthSchemes.TokenServer);
            if (!result.Succeeded)
            {
                return LoginRedirect();
            }
            
            principal = result.Principal;
        }
        else if (request.IsClientCredentialsGrantType())
        {
            var identity = new ClaimsIdentity(AuthSchemes.TokenServer);
            identity.AddClaim(Claims.Subject, request.ClientId!);
            identity.AddClaim(Claims.Role, AuthConsts.Roles.MonteAgent);
            identity.SetDestinations(x => x.Type switch
            {
                Claims.Role when request.HasScope(Scopes.Roles) => new[]
                {
                    Destinations.AccessToken, Destinations.IdentityToken
                },
                _ => new[] { Destinations.IdentityToken }
            });
            principal = new ClaimsPrincipal(identity);
        }
        else
        {
            throw new InvalidOperationException("Invalid grant type.");
        }
        
        return SignIn(principal, AuthSchemes.TokenServer);
    }

    [HttpPost("logout")]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return SignOut(AuthSchemes.TokenServer);
    }
    
    private OpenIddictRequest ExtractRequest()
        => HttpContext.GetOpenIddictServerRequest()
           ?? throw new InvalidOperationException("Invalid OpenIddict request.");
    
    private IActionResult LoginRedirect()
        => Challenge(new AuthenticationProperties
            {
                RedirectUri = _settings.RedirectUri.ToString()
            },
            AuthSchemes.Cookie);
}
