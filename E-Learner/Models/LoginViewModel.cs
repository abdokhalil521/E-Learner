using System.ComponentModel.DataAnnotations;

namespace E_Learner.Models
{
    public class LoginViewModel
    {
        public required string email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}

