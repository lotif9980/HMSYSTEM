using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HMSYSTEM.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        [Required]
        public string? Phone { get; set; }

        public string? BloodGroup { get; set; }
        public string ? Sex { get; set; }
        public string? FatherName { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime  CreateDate { get; set; }

        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string ? Address { get; set; }
        [Required]
        public string ? EmergencyContact { get; set; }
        public string ? Picture { get; set; } = "default.png";
        public bool? Status { get; set; } = true;

        [NotMapped] 
        public IFormFile ImageFile { get; set; }
    }
}
