using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;

namespace Monte.AuthServer.Pages;

[AllowAnonymous]
public class Login : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AuthSettings _settings;

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
    
    public Login(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
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
            return RedirectToPage(_settings.RedirectUri);
        }

        return Page();
    }
}