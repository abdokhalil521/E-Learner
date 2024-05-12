using E_Learner.Data;
using E_Learner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection;

namespace E_Learner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IEmailSender emailSender;

        private readonly ApplicationDbContext _db;

        private readonly IDataProtectionProvider _dataProtectionProvider;



        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, ApplicationDbContext db, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            this.emailSender = emailSender;
            _db = db;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public async Task<IActionResult> verify(string email, string subject, string message)
        {
            email = "mohabelbestawy28@gmail.com";
            await emailSender.SendEmailAsync(email, subject, message);
            return RedirectToAction("verify", "account");
        }

        public IActionResult Index()
        {
            return View();
        }
        /*public IActionResult CreateCookie()
        {
            string key = "My_Cookie";
            string value = "Hello and Welcome";
            CookieOptions co = new CookieOptions();
            co.Expires = DateTime.Now.AddMinutes(1);
            Response.Cookies.Append(key, value, co);
            
                return View("Index");
        }*/

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

        public IActionResult ReadCookie()
        {
            string key = "My_Cookie";
            var cookivalue = Request.Cookies[key];
            return View("Index");
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

        /*public async Task<IActionResult> Logout(Student student)
        {
            // Call the RemoveCookie function to delete the cookie and remove the token from the database
            await RemoveCookie(student);

            // Redirect to the welcome page or any other desired page
            return RedirectToAction("choose", "Home");
        }*/

        public async Task<IActionResult> Logout(TokenViewModel model)
        {
            try
            {
                // Retrieve the student based on the provided email
                var student = await _db.student.FirstOrDefaultAsync(s => s.Email == model.E_mail);

                if (student != null)
                {

                    RemoveCookie(student);
                    // Remove the token from the database
                    /*student.token = "deleted";
                    await _db.SaveChangesAsync();

                    // Set cookie options to expire immediately
                    var options = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(-5),
                        HttpOnly = true
                    };

                    // Remove the cookie from the client's browser
                    Response.Cookies.Delete("My_Cookie", options);*/

                    // Redirect to the welcome page or any other desired page
                    RedirectToAction("choose", "Home");

                    return Ok();

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


        public async Task<IActionResult> RemoveCookie(Student student)
        {
            // Retrieve the token from the student object
            string Tokens = student.token;

            // Remove the token from the database
            student.token = string.Empty;
            await _db.SaveChangesAsync();

            // Set cookie options to expire immediately
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(-5),
                HttpOnly = true
            };

            // Remove the cookie from the client's browser
            Response.Cookies.Delete("My_Cookie", options);

            // Return an Ok result to indicate success
            return Ok();
        }








        public IActionResult choose()
        {
            return View();
        }

        public IActionResult verify()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
