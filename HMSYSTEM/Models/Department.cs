using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department Name Required")]
        public string? DepartmentName { get; set; }
        public bool Status { get; set; }
    }
}
