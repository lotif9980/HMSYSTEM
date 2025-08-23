namespace HMSYSTEM.Models
{
    public class BillDetail
    {
        public int Id { get; set; }
        public int BillId {  get; set; }
        public int ServiceItemId {  get; set; }
        public decimal? Qty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
