﻿using Identity.Domain.ErrorModels;

namespace Identity.WebApi.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public static class ErrorHandle
    {
        public static IActionResult HandleResult(Result result)
        {
            if (result.Succeeded)
            {
                return new OkObjectResult(result);
            }

            var error = result.Errors[0];
            switch (error.StatusCode)
            {
                case ErrorStatusCode.NotFound:
                    return new NotFoundObjectResult(error.Message);

                case ErrorStatusCode.WrongAction:
                    return new BadRequestObjectResult(error.Message);

                case ErrorStatusCode.ForbiddenAction:
                    return new ObjectResult(error.Message) { StatusCode = StatusCodes.Status403Forbidden };

                default:
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }

}
