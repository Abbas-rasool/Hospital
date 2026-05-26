namespace Hospital.Models;

public class Diagnosis
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit? Visit { get; set; }
    public int DoctorId { get; set; }
    public User? Doctor { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string IcdCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
