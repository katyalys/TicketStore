using Identity.Application.Dtos.Mail;

namespace Identity.Application.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(MailDataDto mailData, CancellationToken ct);
    }
}
