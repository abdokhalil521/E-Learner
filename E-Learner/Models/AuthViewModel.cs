using System.ComponentModel.DataAnnotations;

namespace E_Learner.Models
{
    public class AuthViewModel
    {
        
        
            [Required(ErrorMessage = "Verification code is required.")]
            [Display(Name = "Verification Code")]
            public required string Code { get; set; }

            [Display(Name = "Remember Browser")]
            public bool RememberBrowser { get; set; }
        
    }
}
