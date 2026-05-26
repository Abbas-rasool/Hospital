using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly HospitalDbContext _db;
    public DashboardModel(HospitalDbContext db) => _db = db;

    public List<User> Users { get; set; } = new();
    public int TotalUsers { get; set; }
    public int TotalPatients { get; set; }
    public int TodayVisits { get; set; }
    public int TotalMedications { get; set; }

    public async Task OnGetAsync()
    {
        Users = await _db.Users.OrderBy(u => u.Role).ToListAsync();
        TotalUsers = Users.Count;
        TotalPatients = await _db.Patients.CountAsync();
        TodayVisits = await _db.Visits.CountAsync(v => v.Date.Date == DateTime.Today);
        TotalMedications = await _db.Medications.CountAsync(m => m.IsActive);
    }
}
