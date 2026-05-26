using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hospital.Pages.Pharmacy;

[Authorize(Roles = "Admin,Pharmacy,Doctor")]
public class PrintPrescriptionModel : PageModel
{
    private readonly HospitalDbContext _db;
    public PrintPrescriptionModel(HospitalDbContext db) => _db = db;

    public Prescription Prescription { get; set; } = null!;
    public string SolarDate    { get; set; } = string.Empty;
    public int    PatientAge   { get; set; }
    public string DiagnosisNotes { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var rx = await LoadAsync(id);
        if (rx == null) return NotFound();
        Populate(rx);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPrintedAsync(int id)
    {
        var rx = await _db.Prescriptions.FindAsync(id);
        if (rx == null) return NotFound();

        rx.IsPrinted  = true;
        rx.PrintedAt  = DateTime.UtcNow;

        var visit = await _db.Visits.FindAsync(rx.VisitId);
        if (visit != null) visit.Status = VisitStatus.Completed;

        await _db.SaveChangesAsync();
        return new OkResult();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<Prescription?> LoadAsync(int id) =>
        await _db.Prescriptions
            .Include(p => p.CreatedBy)
            .Include(p => p.Visit).ThenInclude(v => v!.Patient)
            .Include(p => p.Visit).ThenInclude(v => v!.Doctor)
            .Include(p => p.Visit).ThenInclude(v => v!.Diagnoses)
            .Include(p => p.Items).ThenInclude(i => i.Medication)
            .FirstOrDefaultAsync(p => p.Id == id);

    private void Populate(Prescription rx)
    {
        Prescription = rx;

        // Solar Hijri date using .NET's built-in PersianCalendar
        var pc   = new PersianCalendar();
        var now  = DateTime.Today;
        SolarDate = $"{pc.GetYear(now)}/{pc.GetMonth(now):D2}/{pc.GetDayOfMonth(now):D2}";

        // Patient age
        if (rx.Visit?.Patient != null)
        {
            var dob = rx.Visit.Patient.DateOfBirth;
            PatientAge = (int)((DateTime.Today - dob).TotalDays / 365.25);
        }

        // Diagnosis from visit
        var dx = rx.Visit?.Diagnoses.OrderByDescending(d => d.CreatedAt).FirstOrDefault();
        DiagnosisNotes = dx?.Notes ?? rx.Notes ?? string.Empty;
    }
}
