using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ErrorModels
{
    public class ErrorClass
    {
        public ErrorClass(ErrorStatusCode statusCode, string message = "")
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ErrorStatusCode StatusCode { get; init; }
        public string Message { get; init; }
    }

    public enum ErrorStatusCode
    {
        WrongAction = 1,
        NotFound = 2,
        ForbiddenAction = 3
    }
}
