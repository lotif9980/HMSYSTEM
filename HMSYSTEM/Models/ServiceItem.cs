namespace HMSYSTEM.Models
{
    public class ServiceItem
    {
        public int Id { get; set; }
        public string ? ItemName {  get; set; }
        public decimal? Amount {  get; set; }
        public bool IsActive { get; set; }
    }
}
