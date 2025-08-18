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
      
        public bool IsOccupied { get; set; }=true;

       
        public Ward? Ward { get; set; }

        [Required]
        public BedType BedType { get; set; }

    }
}
