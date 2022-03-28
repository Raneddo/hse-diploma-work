using MailKit.Net.Smtp;
using MimeKit;

namespace ADSD.Backend.App.Services;

public class EmailService
{
    public void SendRegisterMessage(string userName, string secret, string email)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("ADSD noreply", "noreply@raneddo.ml"));
        mailMessage.To.Add(new MailboxAddress(userName, email));
        mailMessage.Subject = "Hello from ADSD. Registration link below";
        mailMessage.Body = new TextPart("html")
        {
            Text = @$"
<html>
<h2>Hello from ADSD</h2>
Dear {userName}, your email was entered for registration on ADSD conference</br>

Please <a href=""https://wr.raneddo.ml/activate/{secret}"">activate</a> your account.</br>

If you haven't registered to ADSD or you are not {userName}, please remove this email immediately
</html>
"
        };
        
        SendMailMessage(mailMessage);
    }

    public void SendPasswordRecover(string userName, string password, string email)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("ADSD noreply", "noreply@raneddo.ml"));
        mailMessage.To.Add(new MailboxAddress(userName, email));
        mailMessage.Subject = "ADSD. Password recover";
        mailMessage.Body = new TextPart("html")
        {
            Text = @$"
<html>
<h2>Hello from ADSD</h2>
Dear {userName}, you have recovered your password in ADSD</br></br>

Your new credentials:</br>
Username: {userName}</br>
Password: {password}</br>

If you haven't registered to ADSD or you are not {userName}, please remove this email immediately
</html>
"
        };
        
        SendMailMessage(mailMessage);
    }

    private static void SendMailMessage(MimeMessage mailMessage)
    {
        using var smtpClient = new SmtpClient();
        smtpClient.Connect("smtp.mail.ru", 465, true);
        smtpClient.Authenticate("noreply@raneddo.ml", "noreplypass");
        smtpClient.Send(mailMessage);
        smtpClient.Disconnect(true);
    }
}