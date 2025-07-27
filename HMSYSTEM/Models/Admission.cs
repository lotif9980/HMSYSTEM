namespace HMSYSTEM.Models
{
    public class Admission
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int BedId { get; set; }
        public DateTime? AdmitDate { get; set; }
        public int Status { get; set; } = 1;
        //public DateTime? CreateDate {  get; set; }
        public int InvoiceNo { get; set; }



        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Bed Bed { get; set; }
       
    }
}
