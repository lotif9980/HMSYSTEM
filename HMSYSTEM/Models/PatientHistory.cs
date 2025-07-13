namespace HMSYSTEM.Models
{
    public class PatientHistory
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public string? Description { get; set; }
        public string? AttachFile { get; set; }
    }
}
