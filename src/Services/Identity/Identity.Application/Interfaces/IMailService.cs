using Identity.Application.Dtos.MailDto;

namespace Identity.Application.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(MailDataDto mailData, CancellationToken ct);
    }
}
