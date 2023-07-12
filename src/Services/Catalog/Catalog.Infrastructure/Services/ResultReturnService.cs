using Catalog.Domain.ErrorModels;

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
