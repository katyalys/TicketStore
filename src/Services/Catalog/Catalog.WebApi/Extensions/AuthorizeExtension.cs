﻿using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Catalog.WebApi.Extensions
{
    public static class AuthorizeExtension
    {
        public static IServiceCollection AddAuthentificate(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:5012";
                    options.Audience = "Identity WebApi";
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                });
            return services;
        }
    }
}
