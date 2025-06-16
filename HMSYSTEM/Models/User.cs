using System.ComponentModel.DataAnnotations;

namespace HMSYSTEM.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string ? MobileNo { get; set; }
        public string ? UserName { get; set; }
        public string ? Password { get; set; }
        public bool? Status { get; set; }
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
