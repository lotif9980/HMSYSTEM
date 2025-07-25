namespace HMSYSTEM.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public string BedNumber { get; set; }
        public int WardId { get; set; }
        public decimal? RatePerDay {  get; set; }
        public string? BedType {  get; set; }
        public bool IsOccupied { get; set; }

        public Ward? Ward { get; set; }

    }
}
