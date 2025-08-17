using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }

        [Required(ErrorMessage = "Designation Name is required")]
        public string? DesignationName { get; set; }
        public bool Status { get; set; }
    }
}
