using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Catalog.WebApi.Extensions
{
    public static class AuthorizeExtension
    {
        public static IServiceCollection AddAuthentification(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5012";
                    options.Audience = "Identity WebApi";
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    options.RequireHttpsMetadata = false;
                });
            return services;
        }
    }
}
