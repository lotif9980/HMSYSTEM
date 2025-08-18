using HMSYSTEM.Enum;
using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Bed
    {
        public int Id { get; set; }

        [Required (ErrorMessage ="Bed Number Required")]
        public string BedNumber { get; set; }

        
        public int WardId { get; set; }

        [Required]
        public decimal? RatePerDay {  get; set; }
        //public string? BedType {  get; set; }
        public bool IsOccupied { get; set; }=true;

        [Required(ErrorMessage = "Ward Name Required")]
        public Ward? Ward { get; set; }

        [Required]
        public BedType BedType { get; set; }

    }
}
