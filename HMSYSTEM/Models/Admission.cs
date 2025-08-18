using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Admission
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Patient is required")]
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "Patient is required")]
        public int BedId { get; set; }
        public DateTime? AdmitDate { get; set; }
        public int Status { get; set; } = 1;
        //public DateTime? CreateDate {  get; set; }
        public int InvoiceNo { get; set; }
        public string ? AttendentName { get; set; }
        public string ? AttendentRelation { get; set; }
        public string ? AttendentPhone { get;set; }
        public string ? ForReason { get; set; }
        public bool   Declaration { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Bed Bed { get; set; }
       
    }
}
