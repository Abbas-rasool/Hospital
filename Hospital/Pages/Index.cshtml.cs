using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Hospital.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return role switch
        {
            nameof(UserRole.Admin)    => RedirectToPage("/Admin/Dashboard"),
            nameof(UserRole.Registry) => RedirectToPage("/Registry/Patients"),
            nameof(UserRole.Doctor)   => RedirectToPage("/Doctor/Patients"),
            nameof(UserRole.Pharmacy) => RedirectToPage("/Pharmacy/Prescriptions"),
            nameof(UserRole.Lab)      => RedirectToPage("/Lab/Index"),
            _                         => Page()
        };
    }
}
