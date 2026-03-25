using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.Core.Dtos;

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

    public async Task SendAsync(string to, string subject, string body)
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
        await smtp.SendMailAsync(mail);
    }

    public async Task SendLoanApplicationApprovedAsync(LoanApplicationDto application)
    {
        var subject = "Дархости кредити шумо тасдиқ дода шуд! 🎉";
        var body = GenerateLoanApprovalEmailBody(application);
        
        await SendAsync(application.ApplicantEmail, subject, body);
    }

    public async Task SendLoanApplicationRejectedAsync(LoanApplicationDto application)
    {
        var subject = "Маълумот дар бораи дархости кредити шумо";
        var body = GenerateLoanRejectionEmailBody(application);
        
        await SendAsync(application.ApplicantEmail, subject, body);
    }

    public async Task SendLoanApplicationReceivedAsync(LoanApplicationDto application)
    {
        var subject = "Дархости кредити шумо қабул шуд";
        var body = GenerateLoanReceivedEmailBody(application);
        
        await SendAsync(application.ApplicantEmail, subject, body);
    }

    private string GenerateLoanApprovalEmailBody(LoanApplicationDto application)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Тасдиқи Дархости Кредит</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .success {{ color: #28a745; font-weight: bold; }}
        .details {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Табрик! Дархости кредити шумо тасдиқ дода шуд!</h1>
        </div>
        <div class='content'>
            <p>Ирҳоб {application.ApplicantName},</p>
            <p>Мо шодмем, ки ба шумо хабар диҳем, ки дархости кредити шумо аз ҷониби <strong>{application.UserName}</strong> тасдиқ дода шуд.</p>
            
            <div class='details'>
                <h3>Маълумоти дархост:</h3>
                <p><strong>Рақами дархост:</strong> #{application.Id}</p>
                <p><strong>Номи кредит:</strong> {application.CreditName}</p>
                <p><strong>Маблағ:</strong> {application.RequestedAmount:N0} TJS</p>
                <p><strong>Мӯҳлат:</strong> {application.RequestedTermMonths} моҳ</p>
                <p><strong>Санаи тасдиқ:</strong> {application.LastUpdatedDate:dd.MM.yyyy}</p>
            </div>
            
            <p class='success'>Қадамҳои навбатӣ:</p>
            <ol>
                <li>Ба бонк муроҷиат кунед барои гирифтани маблағ</li>
                <li>ҳуҷҷатҳои лозимаро пешкаш кунед</li>
                <li>шартномаро имзо кунед</li>
            </ol>
            
            <p>Агар савол дошта бошед, лутфан ба мо тамос кунед.</p>
        </div>
        <div class='footer'>
            <p>Ин email тавассути системаи CRM Banki фиристода шуд.</p>
            <p>© 2024 Бонки CRM. Ҳамаи ҳуқуқҳо ҳифз шудаанд.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateLoanRejectionEmailBody(LoanApplicationDto application)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Маълумот дар бораи Дархости Кредит</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .info {{ color: #17a2b8; font-weight: bold; }}
        .details {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Маълумот дар бораи дархости кредити шумо</h1>
        </div>
        <div class='content'>
            <p>Ирҳоб {application.ApplicantName},</p>
            <p>Маълумот медиҳем, ки дархости кредити шумо (# {application.Id}) аз ҷониби <strong>{application.UserName}</strong> баррасӣ шуд.</p>
            
            <div class='details'>
                <h3>Маълумоти дархост:</h3>
                <p><strong>Номи кредит:</strong> {application.CreditName}</p>
                <p><strong>Маблағи дархостшуда:</strong> {application.RequestedAmount:N0} TJS</p>
                <p><strong>Мӯҳлат:</strong> {application.RequestedTermMonths} моҳ</p>
                <p><strong>Ҳолати ҳозира:</strong> Рад дода шуд</p>
                {(!string.IsNullOrEmpty(application.RejectionReason) ? $"<p><strong>Сабаби рад:</strong> {application.RejectionReason}</p>" : "")}
            </div>
            
            <p class='info'>Шумо метавонед:</p>
            <ul>
                <li>Дархости навро пешниҳод кунед</li>
                <li>Бо бонк тамос кунед барои маълумоти бештар</li>
                <li>Шартҳоро барои оянда беҳтар кунед</li>
            </ul>
        </div>
        <div class='footer'>
            <p>Ин email тавассути системаи CRM Banki фиристода шуд.</p>
            <p>© 2024 Бонки CRM. Ҳамаи ҳуқуқҳо ҳифз шудаанд.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateLoanReceivedEmailBody(LoanApplicationDto application)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Дархости Кредит Қабул Шуд</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .success {{ color: #28a745; font-weight: bold; }}
        .details {{ background: white; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Дархости кредити шумо қабул шуд!</h1>
        </div>
        <div class='content'>
            <p>Ирҳоб {application.ApplicantName},</p>
            <p>Ташаккур барои муроҷиат ба системаи CRM Banki. Дархости кредити шумо муваффақона қабул шуд ва баррасӣ хоҳад шуд.</p>
            
            <div class='details'>
                <h3>Маълумоти дархост:</h3>
                <p><strong>Рақами дархост:</strong> #{application.Id}</p>
                <p><strong>Номи кредит:</strong> {application.CreditName}</p>
                <p><strong>Маблағ:</strong> {application.RequestedAmount:N0} TJS</p>
                <p><strong>Мӯҳлат:</strong> {application.RequestedTermMonths} моҳ</p>
                <p><strong>Санаи ирсол:</strong> {application.ApplicationDate:dd.MM.yyyy}</p>
            </div>
            
            <p class='success'>Чӣ тавр оянда?</p>
            <ol>
                <li>Дархости шумо аз ҷониби коргарони бонк баррасӣ хоҳад шуд</li>
                <li>Ҳалли масъала ба таври автоматӣ ба email-и шумо фиристода мешавад</li>
            </ol>
            
            <p><strong>Вақти интизорӣ: 1-3 рӯзи корӣ</strong></p>
            
            <p>Агар савол дошта бошед, лутфан ба рақами зерин занг занед:</p>
            <p><strong>Телефон:</strong> +992 123 456 789</p>
            <p><strong>Email:</strong> info@crm-bank.tj</p>
        </div>
        <div class='footer'>
            <p>Ин email тавассути системаи CRM Banki фиристода шуд.</p>
            <p>&copy; 2024 Бонки CRM. Ҳамаи ҳуқуқҳо ҳифз шудаанд.</p>
        </div>
    </div>
</body>
</html>";
    }
}
