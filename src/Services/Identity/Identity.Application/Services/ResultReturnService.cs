using Identity.Domain.ErrorModels;

namespace Identity.Application.Services
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

    }
}
