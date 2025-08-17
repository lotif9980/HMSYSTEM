using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Name Is Required")]
        public string? Name { get; set; }
        public string? GenericName { get; set; }
        public string? Strength { get; set; }
        public string? Form { get; set; }
        public bool? IsActive { get; set; }

    }
}
