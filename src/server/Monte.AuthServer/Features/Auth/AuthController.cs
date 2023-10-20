using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;

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

        if (request.HasPrompt(OpenIddictConstants.Prompts.Login))
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
            new(OpenIddictConstants.Claims.Subject, result.Principal.Identity?.Name
                ?? throw new InvalidOperationException("No identity found."))
        };

        var identity = new ClaimsIdentity(claims, AuthSchemes.Token);
        var principal = new ClaimsPrincipal(identity);

        var scopes = request.GetScopes();
        principal.SetScopes(scopes);

        return SignIn(principal, AuthSchemes.Token);
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = ExtractRequest();

        ClaimsPrincipal principal;
        if (request.IsAuthorizationCodeGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(AuthSchemes.Token);
            if (!result.Succeeded)
            {
                return LoginRedirect();
            }
            
            principal = result.Principal;
        }
        else if (request.IsClientCredentialsGrantType())
        {
            var identity = new ClaimsIdentity(AuthSchemes.Token);

            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId!);

            principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());
        }
        else
        {
            throw new InvalidOperationException("Invalid grant type.");
        }
        
        return SignIn(principal, AuthSchemes.Token);
    }

    [HttpPost("logout")]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return SignOut(AuthSchemes.Token);
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