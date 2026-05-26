namespace Hospital.Models;

public class LabResult
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit? Visit { get; set; }
    public int? LabUserId { get; set; }
    public User? LabUser { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string ReferenceRange { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Notes { get; set; } = string.Empty;
}
