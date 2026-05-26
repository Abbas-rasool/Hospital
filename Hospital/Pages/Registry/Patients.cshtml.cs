using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages.Registry;

[Authorize(Roles = "Admin,Registry")]
public class PatientsModel : PageModel
{
    private readonly HospitalDbContext _db;
    public PatientsModel(HospitalDbContext db) => _db = db;

    public List<Patient> Patients { get; set; } = new();
    public string Query { get; set; } = string.Empty;

    public async Task OnGetAsync(string? q)
    {
        Query = q ?? string.Empty;
        var query = _db.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Query))
        {
            var lower = Query.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(lower) ||
                p.LastName.ToLower().Contains(lower) ||
                p.MRN.Contains(lower) ||
                p.Phone.Contains(lower));
        }

        Patients = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
}
