namespace Hospital.Models;

public enum VisitStatus { Registered, WithDoctor, PrescriptionReady, Completed }

public class Visit
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int RegistryUserId { get; set; }
    public User? RegistryUser { get; set; }
    public int? DoctorId { get; set; }
    public User? Doctor { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Registered;
    public string ChiefComplaint { get; set; } = string.Empty;
    public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}
