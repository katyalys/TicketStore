using Identity.Application.Dtos.MailDto;
using Identity.Application.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;

namespace Identity.Application.Services
{
    public class MailService : Interfaces.IMailService
    {
        private readonly MailSettingsDto _mailSettings;
        public MailService(IOptions<MailSettingsDto> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMailAsync(MailDataDto mailData, CancellationToken ct = default)
        {
            // Initialize a new instance of the MimeKit.MimeMessage class
            var mail = new MimeMessage();

            // Sender
            mail.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            foreach (string mailAddress in mailData.ToMail)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            // Receiver
            foreach (string mailAddress in mailData.ToMail)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            // Add Content to Mime Message
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
