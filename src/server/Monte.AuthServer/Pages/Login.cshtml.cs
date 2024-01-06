using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Data;

namespace Monte.AuthServer.Pages;

[AllowAnonymous]
public class Login : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AuthSettings _settings;

    [FromForm]
    [Required]
    public string Username { get; set; } = null!;

    [FromForm]
    [Required]
    public string Password { get; set; } = null!;
    
    public Login(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IOptions<AuthSettings> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _settings = options.Value;
    }
    
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByNameAsync(Username);
        if (user is null)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(user, Password, false, false);
        if (result.Succeeded)
        {
            return Redirect(_settings.RedirectUri.ToString());
        }

        return Page();
    }
}
