using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.ViewModels
{
    public class AdmissionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        
        [Required(ErrorMessage = "Doctor is required")]
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }

        [Required(ErrorMessage = "Bed is required")]
        public int BedId { get; set; }
        public string BedName { get; set; }

        [Required(ErrorMessage = "Admit date is required")]
        public DateTime? AdmitDate { get; set; }

        public int Status { get; set; } = 1;
        public int InvoiceNo { get; set; }

        [Required(ErrorMessage = "Attendant name is required")]
        public string? AttendentName { get; set; }

        [Required(ErrorMessage = "Attendant relation is required")]
        public string? AttendentRelation { get; set; }

        [Required(ErrorMessage = "Attendant phone is required")]
        public string? AttendentPhone { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        public string? ForReason { get; set; }

        [Required(ErrorMessage = "Declaration is required")]
        public bool Declaration { get; set; }
    }
}
