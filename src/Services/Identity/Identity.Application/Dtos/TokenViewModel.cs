using System.Net;

namespace Identity.Application.Dtos
{
    public class TokenViewModel
    {
        public string AccessToken { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
