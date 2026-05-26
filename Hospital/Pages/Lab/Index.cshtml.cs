using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Hospital.Pages.Lab;

[Authorize(Roles = "Admin,Lab")]
public class IndexModel : PageModel
{
    public void OnGet() { }
}
