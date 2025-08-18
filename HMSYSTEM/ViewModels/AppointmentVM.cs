using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.ViewModels
{
    public class AppointmentVM
    {
        public int AppointmentId { get; set; }
        [Required]
        public string PatientName { get; set; }
        public string? PatientPhoneNumber { get; set; }
        [Required]
        public int? DepartmentId { get; set; }
        [Required]
        public int? DoctorId { get; set; }
        [Required]
        public DateTime? AppoinmentDate { get; set; }
        [Required]
        public int? SerialNumber { get; set; }
      
        public string? Problem { get; set; }


        public Department? Department { get; set; }
        public Doctor? Doctor { get; set; }
        public AppointmentStatus Status { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient? Patient { get; set; }
    }
}
