using System.Net;
using System.Net.Mail;
using tzWepApi.Interfaces;

namespace tzWepApi.Services;

public class EmailSender : IEmailSender
{
    //smtp 
    private string _mail = "romanlistsender@gmail.com";
    private string _password = "ddbyxvdjftkcawdj";
    private int _port = 587;
    
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", _port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_mail, _password)
        };

        return client.SendMailAsync(
            new MailMessage(from: _mail,
                to: email,
                subject,
                message
            ));
    }
}