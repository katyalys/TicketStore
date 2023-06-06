using Identity.Domain;
using Microsoft.AspNetCore.Identity;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IdentityServer;
using Identity.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityDbContext = Identity.Infrastructure.Data.IdentityDbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Duende.IdentityServer.Validation;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity.WebApi.Extensions
{
    public static class IdentityServerExtension
    {
        public static void AddIdentityServerConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole>();

            //services.AddDefaultIdentity<UserManager>()
            //       .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI();


            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(IdentityConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(IdentityConfiguration.GetApiResources())
                .AddInMemoryApiScopes(IdentityConfiguration.GetApiScopes())
                .AddInMemoryClients(IdentityConfiguration.GetClients())
                .AddAspNetIdentity<IdentityUser>();

            services.AddTransient<IProfileService, IdentityProfileService>();
           // services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequireDigit = true;
               // config.Password.RequiredLength = 20;
                config.Password.RequireUppercase = true;
                config.Password.RequireNonAlphanumeric = false;
            });

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 //options.TokenValidationParameters = new TokenValidationParameters()
                 //{
                 //    //ValidateIssuer = false,
                 //    //ValidateAudience = false,
                 //    RoleClaimType = ClaimTypes.Role

                 //};
                 options.Authority = configuration["ID4:Authority"];
                 options.Audience = configuration["ID4:Audience"];
                 options.RequireHttpsMetadata = false;
                 //options.TokenValidationParameters = new TokenValidationParameters()
                 //{
                 //    //ValidateIssuer = false,
                 //    //ValidateAudience = false,
                 //    RoleClaimType = ClaimTypes.Role

                 //};

             });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminOnly", policy =>
            //    {
            //        policy.RequireRole("Admin");
            //    });
            //});

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(Policies.AdminOnly, policy =>
            //    {
            //        policy.RequireRole(Roles.AdminRole.Name);
            //    });
            //});
            // services.AddAuthorization(options => options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin")));
        }
    }
}
