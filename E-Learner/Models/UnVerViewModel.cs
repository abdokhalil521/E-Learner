using System.ComponentModel.DataAnnotations;

namespace E_Learner.Models
{
    public class UnVerViewModel
    {
        public string F_name { get; set; }

        public string L_name { get; set; }

        public string EM_ail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
