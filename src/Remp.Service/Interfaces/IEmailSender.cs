namespace Remp.Service.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string receiver, string title, string body);
}