namespace HMSYSTEM.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public int? ScheduleId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? AppoinmentDate { get;set;}
        public int? SerialNumber { get; set; }
        public string ? Problem { get; set; }
        public bool Status { get; set; }
    }
}
