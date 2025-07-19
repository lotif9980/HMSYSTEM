namespace HMSYSTEM.Models
{
    public class PrescriptionDetail
    {
        public int Id { get; set; }              
        public int PrescriptionId { get; set; }     
        public int? MedicineId { get; set; }
       

        public string? Dose { get; set; }          
        public string? Duration { get; set; }     
        public bool? Instructions { get; set; }

        public Medicine? Medicine { get; set; }
    }
}
