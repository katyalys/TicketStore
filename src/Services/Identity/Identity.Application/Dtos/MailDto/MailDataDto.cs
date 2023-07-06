namespace Identity.Application.Dtos.MailDto
{
    public class MailDataDto
    {
        public List<string> ToMail { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }
    }
}
