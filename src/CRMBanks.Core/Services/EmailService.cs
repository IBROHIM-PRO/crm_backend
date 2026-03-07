using System.Net;
using System.Net.Mail;
using CRMBanks.Core.Services.Interfaces;

namespace CRMBanks.Infrastructure.Services;

public class EmailService : IEmailService
{
    private const string SmtpAddress = "smtp.gmail.com";
    private const int PortNumber = 587;
    private const string FromAddress = "crmbanki@gmail.com";
    private const string Password = "ntom egia ffoy eipn";

    public void Send(string to, string subject, string body)
    {
        using var mail = new MailMessage();
        mail.From = new MailAddress(FromAddress);
        mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        using var smtp = new SmtpClient(SmtpAddress, PortNumber);
        smtp.Credentials = new NetworkCredential(FromAddress, Password);
        smtp.EnableSsl = true;
        smtp.Send(mail);
    }
}
