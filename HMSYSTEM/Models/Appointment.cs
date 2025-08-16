using HMSYSTEM.Enum;

namespace HMSYSTEM.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime AppoinmentDate { get; set; }
        public int? SerialNumber { get; set; }
        public string ? Problem { get; set; }
       

        public Department? Department { get; set; }
        public Doctor? Doctor { get; set; }
        public AppointmentStatus Status { get; set; }


        public int PatientID { get; set; }
        public Patient? Patient { get; set; }
    }
}
