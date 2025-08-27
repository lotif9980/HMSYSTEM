using System.ComponentModel.DataAnnotations.Schema;

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
        public decimal? PaymentAmt{get;set; }
        public decimal? DueAmount { get;set; }
    

        public int? Status { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreateDate { get; set; }
        public string ? Note { get; set; }
        public Patient Patient { get; set; }

        public List<BillDetail> BillDetails { get; set; } = new();

    }
}
