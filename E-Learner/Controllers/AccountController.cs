using E_Learner.Data;
using E_Learner.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_Learner.Services;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json.Linq;

namespace E_Learner.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly MyCookieService _cookieService;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public AccountController(ILogger<AccountController> logger, ApplicationDbContext db, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _db = db;
            _dataProtectionProvider = dataProtectionProvider;
        }
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _db.student.FirstOrDefaultAsync(s => s.Email == model.email);

                    if (student != null && VerifyPassword(model.Password, student.password))
                    {
                        // Authentication successful, create and store token
                        await CreateCookie(student);

                        // Redirect to home page
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the login request.");
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> CreateCookie(Student student)
        {
            // Generate a random token
            string token = GenerateRandomString(10);

            // Encrypt the token
            IDataProtector protector = _dataProtectionProvider.CreateProtector(typeof(AccountController).FullName);
            string encryptedToken = protector.Protect(token);

            // Set cookie options
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5),
                HttpOnly = true // Ensure the cookie is accessible only through HTTP
            };

            // Set the encrypted token as a cookie
            Response.Cookies.Append("My_Cookie", encryptedToken, options);

            // Store the token in the database
            student.token = token;
            await _db.SaveChangesAsync(); // Save changes to the database

            // Return an Ok result to indicate success
            return Ok();
        }



        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /*public async Task<IActionResult> Logout(string token)
        {
            // Call the RemoveCookie function to delete the cookie and remove the token from the database
            await RemoveCookie(token);

            // Redirect to the welcome page or any other desired page
            return RedirectToAction("choose", "Home");
        }*/

        public async Task<IActionResult> Logout(string email)
        {
            try
            {
                // Retrieve the student based on the provided email
                var student = await _db.student.FirstOrDefaultAsync(s => s.Email == email);

                if (student != null)
                {
                    // Remove the token from the database
                    student.token = null;
                    await _db.SaveChangesAsync();

                    // Set cookie options to expire immediately
                    var options = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true
                    };

                    // Remove the cookie from the client's browser
                    Response.Cookies.Delete("My_Cookie", options);

                    // Redirect to the welcome page or any other desired page
                    return RedirectToAction("choose", "Home");
                }
                else
                {
                    // Student not found in the database
                    ModelState.AddModelError(string.Empty, "Student not found.");
                    return View("Error"); // Redirect to an error page or another appropriate action
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the logout request.");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                return View("Error"); // Redirect to an error page or another appropriate action
            }
        }


        /*public IActionResult RemoveCookie()
        {
            string key = "My_Cookie";
            string value = string.Empty;
            CookieOptions co = new CookieOptions();
            co.Expires = DateTime.Now.AddMinutes(-5);
            Response.Cookies.Append(key, value, co);
            return View("choose");
        }*/
        public async Task<IActionResult> RemoveCookie(string token)
        {
            // Retrieve the authenticated user based on the token from the database
            var user = await _db.student.FirstOrDefaultAsync(s => s != null && s.token == token);

            if (user != null)
            {
                // Remove the token from the database
                user.token = null;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Set cookie options to expire immediately
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };

                // Remove the cookie from the client's browser
                Response.Cookies.Delete("My_Cookie", options);

                return RedirectToAction("choose"); // Redirect to a suitable page after logout
            }
            else
            {
                // Token not found in the database, possibly invalid or already expired
                // Log the warning and redirect to an error page
                _logger.LogWarning($"Token {token} not found in the database or is already expired.");
                return RedirectToAction("Error"); // Redirect to an error page or another appropriate action
            }
        }






        /*public IActionResult Logout()
        {
            // Flush token from session upon logout
            HttpContext.Session.Remove("JwtToken");

            // Redirect to choice page where the user can choose to login or signup
            return RedirectToAction("Choice", "Home");
        }*/

        /*private string GenerateJwtToken(string userEmail)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourApp",
                audience: "YourAppClient",
                claims: new[] { new Claim(ClaimTypes.Email, userEmail) },
                expires: DateTime.Now.AddMinutes(30), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/

        /*private string GenerateJwtToken(string email)
        {
            throw new NotImplementedException();
        }*/

        public async Task<IActionResult> Register(UnVerViewModel model)
        {
            /*var body = "hello mail";
            var EmailTest = new MimeMessage();
            EmailTest.From.Add(MailboxAddress.Parse("keira.kuhic@ethereal.email"));
            EmailTest.To.Add(MailboxAddress.Parse("bidoehab521@gmail.com"));
            EmailTest.Subject = "test 1";
            EmailTest.Body = new TextPart(TextFormat.Text) { Text = body };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("keira.kuhic@ethereal.email", "wpCk4rv3CgVZgZ8n8M");
            smtp.Send(EmailTest);
            smtp.Disconnect(true);*/ 
            
            if (ModelState.IsValid)
            {
                var emailAddress = model.EM_ail;
                if (IsValidEmailAddress(emailAddress))
                {



                    var existingUser = await _db.unverified.FirstOrDefaultAsync(s => s.E_Mail == model.EM_ail);

                    if (existingUser != null)
                    {
                        // User already exists
                        ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                        return View(model);
                    }
                    // Hash the password
                    string hashedPassword = HashPassword(model.Password);

                    // Create new student record
                    var Unver = new UnVerified
                    {
                        E_Mail = model.EM_ail,
                        FName = model.F_name,
                        LName = model.L_name,
                        Pass = hashedPassword
                    };

                    _db.unverified.Add(Unver);
                    await _db.SaveChangesAsync();

                    //var sendtoverf = await existingUser.SendEmailAsync("recipient@example.com", "Test Subject", "Test Message");
                    //existingUser.SendEmail("bidoehab521@gmail.com", "Test Email", "Hello, this is a test email!");


                    // Redirect to a confirmation page or log the user in, depending on your flow
                    return RedirectToAction("Index", "Home"); // Adjust as needed
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email address format. Please enter a valid email address.");
                    return View(model);
                }
            }
            else
            {
                // Return view with validation summary or specific error messages
                return View(model);
            }
        }
        public IActionResult Verify()
        {
            return View();
        }

        /*public async Task<IActionResult> VerifyEmail(RegisterViewModel model)
        {
           
        }*/
        /*public IActionResult Logout()
        {
            // Call the RemoveCookie function to delete the cookie
            RemoveCookie();

            // Redirect to the welcome page or any other desired page
            return RedirectToAction("choose", "HomeController");
        }
        public IActionResult RemoveCookie()
        {
            string key = "My_Cookie";
            string value = string.Empty;
            CookieOptions co = new CookieOptions();
            co.Expires = DateTime.Now.AddMinutes(-5);
            Response.Cookies.Append(key, value, co);
            return View("choose");
        }*/
        public IActionResult ReadCookie()
        {
            string key = "My_Cookie";
            var cookivalue = Request.Cookies[key];
            return View("Index");
        }
        

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                var emailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mohsen.bazmi@live.com", "pass")
            };

            return client.SendMailAsync(
                new MailMessage(from: "mohsen.bazmi@live.com",
                                to: email,
                                subject,
                                message
                                ));
        }
        public void SendEmail(string toAddress, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("keira.kuhic@ethereal.email"));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Plain) { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // if you want to choose spesific smtp like gmail, inside the quotations it should look like this smtp.Connect("smtp.gmail.com")
                // if its hot-mail it needs to be ("smtp.live.com")
                // if its office it should be like this ("smtp.office365.com")
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("keira.kuhic@ethereal.email", "wpCk4rv3CgVZgZ8n8M");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
