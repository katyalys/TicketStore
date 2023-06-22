using Catalog.Domain.ErrorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public static class ResultReturnService
    {
        public static Result<T> CreateErrorResult<T>(ErrorStatusCode statusCode, string errorMessage)
        {
            return new Result<T>()
            {
                Errors = new List<ErrorClass>
                {
                    new ErrorClass(statusCode, errorMessage)
                }
            };
        }

        public static Result CreateSuccessResult()
        {
            return new Result();
        }

        public static Result CreateErrorResult(ErrorStatusCode statusCode, string errorMessage)
        {
            return new Result()
            {
                Errors = new List<ErrorClass>
                {
                    new ErrorClass(statusCode, errorMessage)
                }
            };
        }

    }
}
