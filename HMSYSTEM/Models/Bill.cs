namespace HMSYSTEM.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public DateTime BillDate { get; set; }
        public string BillNo { get; set; }
        public int ? PatientId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }

        public Patient Patient { get; set; }

    }
}
