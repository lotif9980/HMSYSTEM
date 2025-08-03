using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HMSYSTEM.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }  
        public string? Phone { get; set; }
        public string? BloodGroup { get; set; }
        public string ? Sex { get; set; }
        public string? FatherName { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime  CreateDate { get; set; }



        public DateTime? DateOfBirth { get; set; }
        public string ? Address { get; set; }
        public string ? EmergencyContact { get; set; }
        public string ? Picture { get; set; } 
        public bool ? Status { get; set; }

        [NotMapped] 
        public IFormFile ImageFile { get; set; }
    }
}
