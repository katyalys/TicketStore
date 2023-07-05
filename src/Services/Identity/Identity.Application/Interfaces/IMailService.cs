using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit;
using Identity.Application.Dtos.MailDto;

namespace Identity.Application.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(MailDataDto mailData, CancellationToken ct);
    }
}
