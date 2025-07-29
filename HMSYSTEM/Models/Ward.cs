using HMSYSTEM.Enum;

namespace HMSYSTEM.Models
{
    public class Ward
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public int ? Type { get; set; }
        public int DepartmentId { get; set; }
        public string ? FloorNo {  get; set; }
        public int ? TotalBeds { get; set; }

        public Department Department { get; set; }
        public WardType Type { get; set; }

        public ICollection<Bed> Bed { get; set; }
    }
}
