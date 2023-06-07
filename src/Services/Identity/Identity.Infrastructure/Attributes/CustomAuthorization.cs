using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Identity.Infrastructure.Attributes
{
    public class CustomAuthorization: Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool authorized = false;

            if (context.HttpContext.User.IsInRole("Customer") || context.HttpContext.User.IsInRole("Admin"))
            {
                authorized = true;
            }
        }

    }
}
