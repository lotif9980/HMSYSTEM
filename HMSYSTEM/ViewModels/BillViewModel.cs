using HMSYSTEM.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.ViewModels
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public DateTime BillDate { get; set; }
        public string BillNo { get; set; }
        public int BedId { get; set; }
        public int? PatientId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? PaymentAmt { get; set; }
        public decimal? DueAmount { get; set; }
        public int? Status { get; set; } = 0;
        public string? Note { get; set; }
        public string ? PatientName {  get; set; }
        public string ? PatientPhoneNumber {  get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreateDate { get; set; }

        public List<BillDetailViewModel> BillDetail { get; set; }=new List<BillDetailViewModel>();

    }

    public class BillDetailViewModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int? ServiceItemId { get; set; }
        public string? ItemName { get; set; }
        public int? Qty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
