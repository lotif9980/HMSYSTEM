namespace HMSYSTEM.Models
{
    public class BillDetail
    {
        public int Id { get; set; }
        public int BillId {  get; set; }
        public int? ServiceItemId {  get; set; }
        public int? Qty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? ChargeDate { get; set; }
    }
}
