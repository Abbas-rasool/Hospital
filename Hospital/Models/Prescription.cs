namespace Hospital.Models;

public class Prescription
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit? Visit { get; set; }
    public int CreatedByUserId { get; set; }
    public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPrinted { get; set; } = false;
    public DateTime? PrintedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}
