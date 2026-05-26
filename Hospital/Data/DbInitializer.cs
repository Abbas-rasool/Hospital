using Hospital.Models;

namespace Hospital.Data;

public static class DbInitializer
{
    public static void Initialize(HospitalDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Username = "admin",    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),    FullName = "System Administrator", Role = UserRole.Admin },
                new User { Username = "registry", PasswordHash = BCrypt.Net.BCrypt.HashPassword("registry123"), FullName = "Registry Staff",       Role = UserRole.Registry },
                new User { Username = "doctor",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("doctor123"),   FullName = "Dr. Default",          Role = UserRole.Doctor },
                new User { Username = "pharmacy", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pharmacy123"), FullName = "Pharmacy Staff",        Role = UserRole.Pharmacy },
                new User { Username = "lab",      PasswordHash = BCrypt.Net.BCrypt.HashPassword("lab123"),      FullName = "Lab Technician",        Role = UserRole.Lab }
            );
        }

        if (!context.Medications.Any())
        {
            context.Medications.AddRange(SeedMedications());
            context.SaveChanges();
        }

        if (!context.Patients.Any())
        {
            var registryUser = context.Users.First(u => u.Role == UserRole.Registry);
            var doctor       = context.Users.First(u => u.Role == UserRole.Doctor);

            var patient = new Patient
            {
                MRN          = "MRN-0001",
                FirstName    = "شکریه",
                LastName     = "احمدی",
                FatherName   = "محمد",
                DateOfBirth  = new DateTime(1997, 1, 1),
                Gender       = "Female",
                Phone        = "0700000000",
                Address      = "Kabul",
                BloodType    = "O+",
                Allergies    = "",
                NationalId   = "12345678",
                CreatedByUserId = registryUser.Id
            };
            context.Patients.Add(patient);
            context.SaveChanges();

            context.Visits.Add(new Visit
            {
                PatientId      = patient.Id,
                RegistryUserId = registryUser.Id,
                DoctorId       = doctor.Id,
                ChiefComplaint = "General check-up",
                Status         = VisitStatus.Registered
            });
            context.SaveChanges();
        }
    }

    private static IEnumerable<Medication> SeedMedications() => new[]
    {
        // Antibiotics
        new Medication { Name = "Amoxicillin",            GenericName = "Amoxicillin",                   Strength = "500mg",        DosageForm = "Capsule",     Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Amoxicillin",            GenericName = "Amoxicillin",                   Strength = "250mg/5ml",    DosageForm = "Syrup",       Category = "Antibiotic",              Unit = "ml" },
        new Medication { Name = "Azithromycin",           GenericName = "Azithromycin",                  Strength = "500mg",        DosageForm = "Tablet",      Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Ciprofloxacin",          GenericName = "Ciprofloxacin",                 Strength = "500mg",        DosageForm = "Tablet",      Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Ciprofloxacin",          GenericName = "Ciprofloxacin",                 Strength = "200mg/100ml",  DosageForm = "IV Infusion", Category = "Antibiotic",              Unit = "ml" },
        new Medication { Name = "Metronidazole",          GenericName = "Metronidazole",                 Strength = "400mg",        DosageForm = "Tablet",      Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Metronidazole",          GenericName = "Metronidazole",                 Strength = "500mg/100ml",  DosageForm = "IV Infusion", Category = "Antibiotic",              Unit = "ml" },
        new Medication { Name = "Ceftriaxone",            GenericName = "Ceftriaxone",                   Strength = "1g",           DosageForm = "Injection",   Category = "Antibiotic",              Unit = "g"  },
        new Medication { Name = "Doxycycline",            GenericName = "Doxycycline",                   Strength = "100mg",        DosageForm = "Capsule",     Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Cloxacillin",            GenericName = "Cloxacillin",                   Strength = "500mg",        DosageForm = "Capsule",     Category = "Antibiotic",              Unit = "mg" },
        new Medication { Name = "Erythromycin",           GenericName = "Erythromycin",                  Strength = "500mg",        DosageForm = "Tablet",      Category = "Antibiotic",              Unit = "mg" },

        // Analgesics
        new Medication { Name = "Paracetamol",            GenericName = "Acetaminophen",                 Strength = "500mg",        DosageForm = "Tablet",      Category = "Analgesic",               Unit = "mg" },
        new Medication { Name = "Paracetamol",            GenericName = "Acetaminophen",                 Strength = "125mg/5ml",    DosageForm = "Syrup",       Category = "Analgesic",               Unit = "ml" },
        new Medication { Name = "Paracetamol",            GenericName = "Acetaminophen",                 Strength = "1g/100ml",     DosageForm = "IV Infusion", Category = "Analgesic",               Unit = "ml" },
        new Medication { Name = "Ibuprofen",              GenericName = "Ibuprofen",                     Strength = "400mg",        DosageForm = "Tablet",      Category = "NSAID",                   Unit = "mg" },
        new Medication { Name = "Ibuprofen",              GenericName = "Ibuprofen",                     Strength = "200mg/5ml",    DosageForm = "Syrup",       Category = "NSAID",                   Unit = "ml" },
        new Medication { Name = "Aspirin",                GenericName = "Acetylsalicylic Acid",          Strength = "75mg",         DosageForm = "Tablet",      Category = "Antiplatelet",            Unit = "mg" },
        new Medication { Name = "Aspirin",                GenericName = "Acetylsalicylic Acid",          Strength = "300mg",        DosageForm = "Tablet",      Category = "NSAID",                   Unit = "mg" },
        new Medication { Name = "Tramadol",               GenericName = "Tramadol HCl",                  Strength = "50mg",         DosageForm = "Capsule",     Category = "Opioid Analgesic",        Unit = "mg" },
        new Medication { Name = "Diclofenac",             GenericName = "Diclofenac Sodium",             Strength = "50mg",         DosageForm = "Tablet",      Category = "NSAID",                   Unit = "mg" },
        new Medication { Name = "Diclofenac",             GenericName = "Diclofenac Sodium",             Strength = "75mg/3ml",     DosageForm = "Injection",   Category = "NSAID",                   Unit = "mg" },

        // Cardiovascular
        new Medication { Name = "Metoprolol",             GenericName = "Metoprolol Tartrate",           Strength = "50mg",         DosageForm = "Tablet",      Category = "Beta-Blocker",            Unit = "mg" },
        new Medication { Name = "Amlodipine",             GenericName = "Amlodipine Besylate",           Strength = "5mg",          DosageForm = "Tablet",      Category = "Calcium Channel Blocker", Unit = "mg" },
        new Medication { Name = "Amlodipine",             GenericName = "Amlodipine Besylate",           Strength = "10mg",         DosageForm = "Tablet",      Category = "Calcium Channel Blocker", Unit = "mg" },
        new Medication { Name = "Enalapril",              GenericName = "Enalapril Maleate",             Strength = "5mg",          DosageForm = "Tablet",      Category = "ACE Inhibitor",           Unit = "mg" },
        new Medication { Name = "Furosemide",             GenericName = "Furosemide",                    Strength = "40mg",         DosageForm = "Tablet",      Category = "Diuretic",                Unit = "mg" },
        new Medication { Name = "Furosemide",             GenericName = "Furosemide",                    Strength = "10mg/ml",      DosageForm = "Injection",   Category = "Diuretic",                Unit = "mg" },
        new Medication { Name = "Atorvastatin",           GenericName = "Atorvastatin Calcium",          Strength = "20mg",         DosageForm = "Tablet",      Category = "Statin",                  Unit = "mg" },
        new Medication { Name = "Atorvastatin",           GenericName = "Atorvastatin Calcium",          Strength = "40mg",         DosageForm = "Tablet",      Category = "Statin",                  Unit = "mg" },
        new Medication { Name = "Digoxin",                GenericName = "Digoxin",                       Strength = "0.25mg",       DosageForm = "Tablet",      Category = "Cardiac Glycoside",       Unit = "mg" },
        new Medication { Name = "Warfarin",               GenericName = "Warfarin Sodium",               Strength = "5mg",          DosageForm = "Tablet",      Category = "Anticoagulant",           Unit = "mg" },

        // Gastrointestinal
        new Medication { Name = "Omeprazole",             GenericName = "Omeprazole",                    Strength = "20mg",         DosageForm = "Capsule",     Category = "PPI",                     Unit = "mg" },
        new Medication { Name = "Omeprazole",             GenericName = "Omeprazole",                    Strength = "40mg",         DosageForm = "Injection",   Category = "PPI",                     Unit = "mg" },
        new Medication { Name = "Metoclopramide",         GenericName = "Metoclopramide HCl",            Strength = "10mg",         DosageForm = "Tablet",      Category = "Antiemetic",              Unit = "mg" },
        new Medication { Name = "Ondansetron",            GenericName = "Ondansetron HCl",               Strength = "4mg",          DosageForm = "Tablet",      Category = "Antiemetic",              Unit = "mg" },
        new Medication { Name = "Ondansetron",            GenericName = "Ondansetron HCl",               Strength = "4mg/2ml",      DosageForm = "Injection",   Category = "Antiemetic",              Unit = "mg" },
        new Medication { Name = "Ranitidine",             GenericName = "Ranitidine HCl",                Strength = "150mg",        DosageForm = "Tablet",      Category = "H2 Blocker",              Unit = "mg" },
        new Medication { Name = "Lactulose",              GenericName = "Lactulose",                     Strength = "3.35g/5ml",    DosageForm = "Syrup",       Category = "Laxative",                Unit = "ml" },
        new Medication { Name = "Hyoscine",               GenericName = "Hyoscine Butylbromide",         Strength = "10mg",         DosageForm = "Tablet",      Category = "Antispasmodic",           Unit = "mg" },
        new Medication { Name = "Hyoscine",               GenericName = "Hyoscine Butylbromide",         Strength = "20mg/ml",      DosageForm = "Injection",   Category = "Antispasmodic",           Unit = "mg" },

        // Respiratory
        new Medication { Name = "Salbutamol",             GenericName = "Salbutamol",                    Strength = "2mg",          DosageForm = "Tablet",      Category = "Bronchodilator",          Unit = "mg" },
        new Medication { Name = "Salbutamol",             GenericName = "Salbutamol",                    Strength = "100mcg/dose",  DosageForm = "Inhaler",     Category = "Bronchodilator",          Unit = "mcg"},
        new Medication { Name = "Prednisolone",           GenericName = "Prednisolone",                  Strength = "5mg",          DosageForm = "Tablet",      Category = "Corticosteroid",          Unit = "mg" },
        new Medication { Name = "Theophylline",           GenericName = "Theophylline",                  Strength = "200mg",        DosageForm = "Tablet",      Category = "Bronchodilator",          Unit = "mg" },
        new Medication { Name = "Bromhexine",             GenericName = "Bromhexine HCl",                Strength = "8mg",          DosageForm = "Tablet",      Category = "Mucolytic",               Unit = "mg" },

        // Diabetes
        new Medication { Name = "Metformin",              GenericName = "Metformin HCl",                 Strength = "500mg",        DosageForm = "Tablet",      Category = "Antidiabetic",            Unit = "mg" },
        new Medication { Name = "Metformin",              GenericName = "Metformin HCl",                 Strength = "1000mg",       DosageForm = "Tablet",      Category = "Antidiabetic",            Unit = "mg" },
        new Medication { Name = "Glibenclamide",          GenericName = "Glibenclamide",                 Strength = "5mg",          DosageForm = "Tablet",      Category = "Antidiabetic",            Unit = "mg" },
        new Medication { Name = "Insulin Regular",        GenericName = "Regular Insulin",               Strength = "100IU/ml",     DosageForm = "Injection",   Category = "Insulin",                 Unit = "IU" },
        new Medication { Name = "Insulin NPH",            GenericName = "Isophane Insulin",              Strength = "100IU/ml",     DosageForm = "Injection",   Category = "Insulin",                 Unit = "IU" },

        // Steroids
        new Medication { Name = "Dexamethasone",          GenericName = "Dexamethasone",                 Strength = "4mg/ml",       DosageForm = "Injection",   Category = "Corticosteroid",          Unit = "mg" },
        new Medication { Name = "Dexamethasone",          GenericName = "Dexamethasone",                 Strength = "0.5mg",        DosageForm = "Tablet",      Category = "Corticosteroid",          Unit = "mg" },
        new Medication { Name = "Hydrocortisone",         GenericName = "Hydrocortisone",                Strength = "100mg",        DosageForm = "Injection",   Category = "Corticosteroid",          Unit = "mg" },

        // Antihistamines
        new Medication { Name = "Cetirizine",             GenericName = "Cetirizine HCl",                Strength = "10mg",         DosageForm = "Tablet",      Category = "Antihistamine",           Unit = "mg" },
        new Medication { Name = "Loratadine",             GenericName = "Loratadine",                    Strength = "10mg",         DosageForm = "Tablet",      Category = "Antihistamine",           Unit = "mg" },
        new Medication { Name = "Chlorphenamine",         GenericName = "Chlorphenamine Maleate",        Strength = "4mg",          DosageForm = "Tablet",      Category = "Antihistamine",           Unit = "mg" },
        new Medication { Name = "Promethazine",           GenericName = "Promethazine HCl",              Strength = "25mg",         DosageForm = "Tablet",      Category = "Antihistamine",           Unit = "mg" },

        // CNS
        new Medication { Name = "Diazepam",               GenericName = "Diazepam",                      Strength = "5mg",          DosageForm = "Tablet",      Category = "Benzodiazepine",          Unit = "mg" },
        new Medication { Name = "Diazepam",               GenericName = "Diazepam",                      Strength = "10mg/2ml",     DosageForm = "Injection",   Category = "Benzodiazepine",          Unit = "mg" },
        new Medication { Name = "Haloperidol",            GenericName = "Haloperidol",                   Strength = "5mg",          DosageForm = "Tablet",      Category = "Antipsychotic",           Unit = "mg" },
        new Medication { Name = "Amitriptyline",          GenericName = "Amitriptyline HCl",             Strength = "25mg",         DosageForm = "Tablet",      Category = "Antidepressant",          Unit = "mg" },
        new Medication { Name = "Phenobarbitone",         GenericName = "Phenobarbital",                 Strength = "30mg",         DosageForm = "Tablet",      Category = "Anticonvulsant",          Unit = "mg" },
        new Medication { Name = "Phenytoin",              GenericName = "Phenytoin Sodium",              Strength = "100mg",        DosageForm = "Capsule",     Category = "Anticonvulsant",          Unit = "mg" },

        // Vitamins & Supplements
        new Medication { Name = "Vitamin C",              GenericName = "Ascorbic Acid",                 Strength = "500mg",        DosageForm = "Tablet",      Category = "Vitamin",                 Unit = "mg" },
        new Medication { Name = "Vitamin D3",             GenericName = "Cholecalciferol",               Strength = "1000IU",       DosageForm = "Capsule",     Category = "Vitamin",                 Unit = "IU" },
        new Medication { Name = "Ferrous Sulfate",        GenericName = "Ferrous Sulfate",               Strength = "200mg",        DosageForm = "Tablet",      Category = "Iron Supplement",         Unit = "mg" },
        new Medication { Name = "Folic Acid",             GenericName = "Folic Acid",                    Strength = "5mg",          DosageForm = "Tablet",      Category = "Vitamin",                 Unit = "mg" },
        new Medication { Name = "Calcium Carbonate",      GenericName = "Calcium Carbonate",             Strength = "500mg",        DosageForm = "Tablet",      Category = "Mineral Supplement",      Unit = "mg" },
        new Medication { Name = "Zinc Sulfate",           GenericName = "Zinc Sulfate",                  Strength = "20mg",         DosageForm = "Tablet",      Category = "Mineral Supplement",      Unit = "mg" },
        new Medication { Name = "B-Complex",              GenericName = "Vitamin B Complex",             Strength = "-",            DosageForm = "Tablet",      Category = "Vitamin",                 Unit = "-"  },

        // Antimalarials
        new Medication { Name = "Artemether/Lumefantrine",GenericName = "Artemether + Lumefantrine",     Strength = "20/120mg",     DosageForm = "Tablet",      Category = "Antimalarial",            Unit = "mg" },
        new Medication { Name = "Chloroquine",            GenericName = "Chloroquine Phosphate",         Strength = "250mg",        DosageForm = "Tablet",      Category = "Antimalarial",            Unit = "mg" },
        new Medication { Name = "Quinine",                GenericName = "Quinine Sulfate",               Strength = "300mg",        DosageForm = "Tablet",      Category = "Antimalarial",            Unit = "mg" },

        // IV Fluids
        new Medication { Name = "Normal Saline",          GenericName = "Sodium Chloride 0.9%",          Strength = "0.9%",         DosageForm = "IV Fluid",    Category = "IV Fluid",                Unit = "ml" },
        new Medication { Name = "Ringer's Lactate",       GenericName = "Lactated Ringer's",             Strength = "-",            DosageForm = "IV Fluid",    Category = "IV Fluid",                Unit = "ml" },
        new Medication { Name = "Dextrose 5%",            GenericName = "Dextrose 5%",                   Strength = "5%",           DosageForm = "IV Fluid",    Category = "IV Fluid",                Unit = "ml" },
        new Medication { Name = "Dextrose-Saline",        GenericName = "Dextrose 5% + Saline 0.45%",   Strength = "5%/0.45%",     DosageForm = "IV Fluid",    Category = "IV Fluid",                Unit = "ml" },
    };
}
