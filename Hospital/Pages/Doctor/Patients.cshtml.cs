using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages.Doctor;

[Authorize(Roles = "Admin,Doctor")]
public class PatientsModel : PageModel
{
    private readonly HospitalDbContext _db;
    public PatientsModel(HospitalDbContext db) => _db = db;

    public List<Visit> Visits { get; set; } = new();

    public async Task OnGetAsync()
    {
        Visits = await _db.Visits
            .Include(v => v.Patient)
            .Where(v => v.Status != VisitStatus.Completed)
            .OrderBy(v => v.Date)
            .ToListAsync();
    }
}
