using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages.Pharmacy;

[Authorize(Roles = "Admin,Pharmacy")]
public class PrescriptionsModel : PageModel
{
    private readonly HospitalDbContext _db;
    public PrescriptionsModel(HospitalDbContext db) => _db = db;

    // Visits that already have a prescription (ready to print or already printed)
    public List<Visit> ReadyVisits { get; set; } = new();

    // Visits with no prescription yet — pharmacy can write one
    public List<Visit> ActiveVisits { get; set; } = new();

    public async Task OnGetAsync()
    {
        var allVisits = await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
            .Include(v => v.Prescriptions).ThenInclude(p => p.CreatedBy)
            .Where(v => v.Status != VisitStatus.Completed)
            .OrderByDescending(v => v.Date)
            .ToListAsync();

        ReadyVisits  = allVisits.Where(v => v.Prescriptions.Any()).ToList();
        ActiveVisits = allVisits.Where(v => !v.Prescriptions.Any()).ToList();
    }
}
