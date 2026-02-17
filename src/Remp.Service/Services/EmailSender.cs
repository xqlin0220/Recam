using System.Net;
using System.Net.Mail;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _port = 587;
    private readonly string _senderEmail = "your_email@gmail.com";
    private readonly string _password = "your_app_password";

    public async Task SendEmailAsync(string receiver, string title, string body)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_senderEmail);
        message.To.Add(receiver);
        message.Subject = title;
        message.Body = body;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(_smtpServer, _port)
        {
            Credentials = new NetworkCredential(_senderEmail, _password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}