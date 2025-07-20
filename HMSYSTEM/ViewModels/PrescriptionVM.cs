using HMSYSTEM.Models;

namespace HMSYSTEM.ViewModels
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }

        public int PatientId { get; set; }
        public string? PatientName { get; set; }

        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int? Status { get; set; }
        public string? Note { get; set; }
        public DateTime? NextFlowUp { get; set; }
       
        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; } = new List<PrescriptionDetailViewModel>();
    }

    public class PrescriptionDetailViewModel
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int? MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public Medicine? Medicine { get; set; }

        public string? Dose { get; set; }
        public string? Duration { get; set; }
        public bool? Instructions { get; set; }
    }

}
