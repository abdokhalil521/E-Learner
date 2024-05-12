using System.ComponentModel.DataAnnotations;
using System.Text;

using System.Security.Cryptography;


namespace E_Learner.Models

{
    public class Student
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required] 
        public string Lastname { get; set; }
        [Required]
        public string Email {get; set;}
        [Required]
        public string password { get; set;}

        public string token { get; set;}

        /*public void SetPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return password == hashedPassword;
            }
        }*/
    }
}
