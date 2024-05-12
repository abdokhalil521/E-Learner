using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;

namespace E_Learner.Controllers
{
    public class EmailController : Controller
    {

        [HttpPost]
        public IActionResult SendEamil(string body) 
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("keira.kuhic@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("keira.kuhic@ethereal.email"));
            email.Subject = "test 1";
            email.Body = new TextPart(TextFormat.Html) { Text = body };


            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            // if you want to choose spesific smtp like gmail, inside the quotations it should look like this smtp.Connect("smtp.gmail.com")
            // if its hot-mail it needs to be ("smtp.live.com")
            // if its office it should be like this ("smtp.office365.com")
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("keira.kuhic@ethereal.email", "wpCk4rv3CgVZgZ8n8M");
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
