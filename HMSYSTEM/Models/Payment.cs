namespace HMSYSTEM.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BillId {  get; set; }
        public DateTime PaymentDate {  get; set; }
        public decimal PaymentAmount {  get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
