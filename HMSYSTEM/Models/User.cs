using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Mobile Number Required")]
        public string ? MobileNo { get; set; }
        [Required]
        public string ? UserName { get; set; }
        [Required]
        public string ? Password { get; set; }
        public bool? Status { get; set; }
        [Required(ErrorMessage ="Role Required")]
        public int ? RoleId { get; set; }
        public Role? Role { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
      
    }

   
}
