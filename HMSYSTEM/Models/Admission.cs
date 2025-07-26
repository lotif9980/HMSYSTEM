namespace HMSYSTEM.Models
{
    public class Admission
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int BedId { get; set; }
        public DateTime AdmitDate { get; set; }
        public int Status  { get; set;}
        public DateTime CreateDate {  get; set; }
    }
}
