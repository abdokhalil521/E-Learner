/*using System.Net.Mail;
using System.Net;

namespace E_Learner.SMTP
{
    public interface ISenderEmail
    {
        Task SendEmailAsync (string ToEmail, string Subject, 
        string Body, Stream attachmentStream, string attachmentName, bool IsBodyHtml = false);
    }

    public class EmailService : ISenderEmail
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public Task SendEmailAsync(string ToEmail, string Subject, string Body, Stream attachmentStream, string attachmentName, bool IsBodyHtml = false)
        {
            throw new NotImplementedException();
        }
    }
}*/

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}
