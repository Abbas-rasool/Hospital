using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Hospital.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly HospitalDbContext _db;
    public LoginModel(HospitalDbContext db) => _db = db;

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = _db.Users.SingleOrDefault(u => u.Username == Username && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("Username", user.Username),
        };

        var identity = new ClaimsIdentity(claims, "HospitalCookies");
        await HttpContext.SignInAsync("HospitalCookies", new ClaimsPrincipal(identity));

        return RedirectToPage("/Index");
    }
}
