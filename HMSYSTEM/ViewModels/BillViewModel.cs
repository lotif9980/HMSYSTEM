namespace HMSYSTEM.ViewModels
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public DateTime BillDate { get; set; }
        public string BillNo { get; set; }
        public int? PatientId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public int? Status { get; set; }

        public List<BillDetailViewModel> BillDetail { get; set; }=new List<BillDetailViewModel>();

    }

    public class BillDetailViewModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int ServiceItemId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
