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

    public List<Prescription> Prescriptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        Prescriptions = await _db.Prescriptions
            .Include(p => p.CreatedBy)
            .Include(p => p.Visit).ThenInclude(v => v!.Patient)
            .Include(p => p.Items).ThenInclude(i => i.Medication)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}
