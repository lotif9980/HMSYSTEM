using HMSYSTEM.Enum;
using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Bed
    {
        public int Id { get; set; }

        [Required]
        public string BedNumber { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public decimal? RatePerDay {  get; set; }
        //public string? BedType {  get; set; }
        public bool IsOccupied { get; set; }=true;

        public Ward? Ward { get; set; }

        [Required]
        public BedType BedType { get; set; }

    }
}
