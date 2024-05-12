using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;


public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = "bidoehab521@gmail.com";
        //var pass = "";
        //var host="";
        //if (email != null && email.Contains("@gmail.com") )
        //{
        //var host = "smtp.gmail.com";
        //var pass = "rcheohjqtjrgzyhl";
        //}else if(email != null && email.Contains("@gmail.com"))
        //{
        var host = "smtp.gmail.com";
        var pass = "xgnoriwzpqfulpqv";
        //}
        var client = new SmtpClient(host, 587)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pass)
        };
        string code = codeGenerate();

        subject = "CleverCamp Verification";
        message = "Thanks for starting the new CleverCamp account creation process." +
            " We want to make sure it's really you." +
            " Please enter the following verification code when prompted." +
            " If you don’t want to create an account," +
            " you can ignore this message.\r\n\r\n" + code +
            "\r\n";
        return client.SendMailAsync(
            new MailMessage(from: mail,
                            to: email,
                            subject,
                            message));


    }
    public string codeGenerate()
    {
        var buffer = new byte[sizeof(UInt64)];
        using (var cryptoRng = new RNGCryptoServiceProvider())
        {
            cryptoRng.GetBytes(buffer);
            var num = BitConverter.ToUInt64(buffer, 0);
            var code = num % 1000000; // Ensure it's a 6-digit number
            return code.ToString("D6"); // Format as a 6-character string
        }
    }
}

