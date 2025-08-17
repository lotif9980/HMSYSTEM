using HMSYSTEM.Enum;
using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class Ward
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //public int ? Type { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string ? FloorNo {  get; set; }

        [Required]
        public int ? TotalBeds { get; set; }

        public Department Department { get; set; }

        [Required]
        public WardType Type { get; set; }

        public ICollection<Bed> Bed { get; set; }
    }
}
