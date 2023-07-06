using Identity.Application.Dtos.MailDto;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Identity.Application.Interfaces;

namespace Identity.Application.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettingsDto _mailSettings;

        public MailService(IOptions<MailSettingsDto> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMailAsync(MailDataDto mailData, CancellationToken ct = default)
        {
            var mail = new MimeMessage();
            mail.Sender = MailboxAddress.Parse(_mailSettings.Mail);

            foreach (string mailAddress in mailData.ToMail)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, ct);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);
        }
    }
}
