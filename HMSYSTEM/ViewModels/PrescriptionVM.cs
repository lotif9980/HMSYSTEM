using HMSYSTEM.Models;

namespace HMSYSTEM.ViewModels
{
    public class PrescriptionViewModel
    {

        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? DepartmentId { get; set; }
        public int? Status { get; set; }
        public string ? Note { get; set; }
        public DateTime? NextFlowUp { get; set; }

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
        public Department? Department { get; set; }
        //public List<PrescriptionDetailViewModel> PrescriptionDetail { get; set; }
        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; } = new List<PrescriptionDetailViewModel>();
    }

    public class PrescriptionDetailViewModel
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public string? MedicineId { get; set; }
        public string? Dose { get; set; }
        public string? Duration { get; set; }
        public bool? Instructions { get; set; }

    }
}
