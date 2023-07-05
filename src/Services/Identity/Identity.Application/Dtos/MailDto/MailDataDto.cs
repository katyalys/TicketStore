using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dtos.MailDto
{
    public class MailDataDto
    {
        public List<string> ToMail { get; set; }

        //public string? FromMail { get; }

        //public string? DisplayName { get; }
        public string Subject { get; set; }

        public string? Body { get; set; }

    }
}
