using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public int ? DesignationId { get; set; }
        public int ? DepartmentId { get; set; }
        public string ? Address { get; set; }
        public string ? PhoneNo { get; set; }  
        public string ? ShortBiography { get; set; }
        public string ? Specialist { get; set; }
        public DateTime ? DateofBirth { get; set; }
        public string? Sex { get; set; }
        public string? Picture { get; set; }
        public bool Status { get; set; }

        public Department? Department { get; set; }
        public Designation? Designation { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }



}
