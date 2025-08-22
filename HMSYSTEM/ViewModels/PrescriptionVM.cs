using HMSYSTEM.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.ViewModels
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }

        [Required]
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientMobileNo { get; set; }
        public String? PatientAddress { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public string? DoctorName { get; set; }
        public string? DoctorMobile { get; set; }

        [Required]
        public int DesignationId { get; set; }
     
        public string? DesignationName { get; set; }

        [Required]
        public int DepartmentId { get; set; }
      
        public string? DepartmentName { get; set; }

        public int? Status { get; set; } = 1;
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
        public string ? Strength { get; set; }
        public string? Form { get; set; }
    }       

}
