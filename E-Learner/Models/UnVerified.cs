using System.ComponentModel.DataAnnotations;

namespace E_Learner.Models
{
    public class UnVerified
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string E_Mail { get; set; }
        [Required]
        public string Pass { get; set; }

        //public string? Token  { get; set; }
    }
}
