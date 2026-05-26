using Hospital.Data;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hospital.Pages.Doctor;

[Authorize(Roles = "Admin,Doctor,Pharmacy")]
public class VisitDetailModel : PageModel
{
    private readonly HospitalDbContext _db;
    public VisitDetailModel(HospitalDbContext db) => _db = db;

    public Visit Visit { get; set; } = null!;
    public int PatientAge { get; set; }
    public int? ExistingPrescriptionId { get; set; }
    public List<PrescriptionItem> ExistingItems { get; set; } = new();

    [BindProperty] public string? DiagnosisNotes { get; set; }
    [BindProperty] public List<RxItemInput> Items { get; set; } = new();

    public static readonly string[] FrequencyOptions =
        ["QD", "BD", "TID", "QID", "q6hr", "q8hr", "q12hr", "Stat", "PRN", "Once"];

    public class RxItemInput
    {
        public int     MedicationId { get; set; }
        public string? DosageForm   { get; set; }
        public string? Frequency    { get; set; }
        public string? Duration     { get; set; }
        public int     Quantity     { get; set; } = 1;
        public string? Notes        { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var visit = await LoadVisitAsync(id);
        if (visit == null) return NotFound();
        PopulateProperties(visit);
        return Page();
    }

    public async Task<JsonResult> OnGetSearchMedicationsAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return new JsonResult(Array.Empty<object>());

        var lower = q.ToLower();
        var meds = await _db.Medications
            .Where(m => m.IsActive &&
                (m.Name.ToLower().Contains(lower) ||
                 m.GenericName.ToLower().Contains(lower) ||
                 m.Category.ToLower().Contains(lower)))
            .Select(m => new { id = m.Id, name = m.Name, genericName = m.GenericName, strength = m.Strength, dosageForm = m.DosageForm, category = m.Category })
            .Take(15)
            .ToListAsync();

        return new JsonResult(meds);
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var visit = await LoadVisitAsync(id);
        if (visit == null) return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Save/update diagnosis
        var existingDx = visit.Diagnoses.OrderByDescending(d => d.CreatedAt).FirstOrDefault();
        if (existingDx != null)
        {
            existingDx.Notes = DiagnosisNotes ?? string.Empty;
        }
        else if (!string.IsNullOrWhiteSpace(DiagnosisNotes))
        {
            visit.Diagnoses.Add(new Diagnosis
            {
                VisitId  = visit.Id,
                DoctorId = userId,
                Notes    = DiagnosisNotes
            });
        }

        // Save prescription
        var validItems = Items.Where(i => i.MedicationId > 0).ToList();
        if (validItems.Any())
        {
            // Replace existing unprinted prescription
            var oldRx = visit.Prescriptions.FirstOrDefault(p => !p.IsPrinted);
            if (oldRx != null)
            {
                _db.PrescriptionItems.RemoveRange(oldRx.Items);
                _db.Prescriptions.Remove(oldRx);
            }

            visit.Prescriptions.Add(new Prescription
            {
                VisitId         = visit.Id,
                CreatedByUserId = userId,
                Notes           = DiagnosisNotes ?? string.Empty,
                Items           = validItems.Select((item, idx) => new PrescriptionItem
                {
                    MedicationId = item.MedicationId,
                    Dosage       = item.DosageForm  ?? string.Empty,
                    Frequency    = item.Frequency   ?? string.Empty,
                    Duration     = item.Duration    ?? string.Empty,
                    Quantity     = item.Quantity,
                    Notes        = item.Notes       ?? string.Empty
                }).ToList()
            });

            visit.Status = VisitStatus.PrescriptionReady;
            if (visit.DoctorId == null) visit.DoctorId = userId;
        }

        await _db.SaveChangesAsync();

        // Redirect each role back to their own section
        return User.IsInRole("Pharmacy")
            ? RedirectToPage("/Pharmacy/Prescriptions")
            : RedirectToPage("/Doctor/Patients");
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<Visit?> LoadVisitAsync(int id) =>
        await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.RegistryUser)
            .Include(v => v.Doctor)
            .Include(v => v.Diagnoses)
            .Include(v => v.Prescriptions).ThenInclude(p => p.Items).ThenInclude(i => i.Medication)
            .Include(v => v.Prescriptions).ThenInclude(p => p.CreatedBy)
            .FirstOrDefaultAsync(v => v.Id == id);

    private void PopulateProperties(Visit visit)
    {
        Visit = visit;
        PatientAge = visit.Patient == null
            ? 0
            : (int)((DateTime.Today - visit.Patient.DateOfBirth).TotalDays / 365.25);

        var rx = visit.Prescriptions.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
        if (rx != null)
        {
            ExistingPrescriptionId = rx.Id;
            ExistingItems = rx.Items.ToList();
        }

        var dx = visit.Diagnoses.OrderByDescending(d => d.CreatedAt).FirstOrDefault();
        if (dx != null) DiagnosisNotes = dx.Notes ?? string.Empty;
    }
}
