using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Data;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<LabResult> LabResults => Set<LabResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Role).HasConversion<string>();

        modelBuilder.Entity<Visit>()
            .Property(v => v.Status).HasConversion<string>();

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.CreatedBy).WithMany()
            .HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.RegistryUser).WithMany()
            .HasForeignKey(v => v.RegistryUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Doctor).WithMany()
            .HasForeignKey(v => v.DoctorId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Diagnosis>()
            .HasOne(d => d.Doctor).WithMany()
            .HasForeignKey(d => d.DoctorId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.CreatedBy).WithMany()
            .HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LabResult>()
            .HasOne(l => l.LabUser).WithMany()
            .HasForeignKey(l => l.LabUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
