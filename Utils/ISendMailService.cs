namespace WebEnterprise.Utils;

public interface ISendMailService
{
    Task SendMail(MailContent mailContent);
    Task SendMailAsync(string email, string subject, string htmlMessage);
}