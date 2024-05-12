using System.ComponentModel.DataAnnotations;

namespace E_Learner.Models
{
    public class RegisterViewModel
    {
        public string firstname { get; set; }

        public string lastname { get; set; }

        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
