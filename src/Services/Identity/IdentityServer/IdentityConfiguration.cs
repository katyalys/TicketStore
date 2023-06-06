using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.IdentityServer
{
    public class IdentityConfiguration
    {
        private readonly IConfiguration _configuration;
        public IdentityConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
                {
                //new Client
                //{
                //    ClientId = "User Identity WebApi",
                //    ClientSecrets = { new Secret("secret".Sha256()) },

                //    AllowedGrantTypes = GrantTypes.Code,
            
                //    // where to redirect to after login
                //    RedirectUris = { "https://localhost:5012/signin-oidc" },

                //    // where to redirect to after logout
                //    PostLogoutRedirectUris = { "https://localhost:5012/signout-callback-oidc" },

                //    AllowOfflineAccess = true,

                //    AllowedScopes = new List<string>
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "view account",
                //    }
                //},

                //new Client
                //{
                //    ClientId = "Admin Identity WebApi",
                //    ClientSecrets = { new Secret("secret".Sha256()) },

                //    AllowedGrantTypes = GrantTypes.Code,
            
                //    // where to redirect to after login
                //    RedirectUris = { "https://localhost:5012/signin-oidc" },

                //    // where to redirect to after logout
                //    PostLogoutRedirectUris = { "https://localhost:5012/signout-callback-oidc" },

                //    AllowOfflineAccess = true,

                //    AllowedScopes = new List<string>
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "delete user",
                //        "change user role",
                //        "view users"
                //    }
                //}

  
                new Client
                {
                    ClientId = "identityUserClient",
                    ClientName = "Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    RedirectUris           = { "http://localhost:5012/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5012/signout-callback-oidc" },

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("IdentityApiSecret".Sha256())
                    },
                    AllowedScopes = new List<string> { "Identity.Offline_access", "Identity", "IdentityServerApi"},
                    AllowOfflineAccess = true,
                    AccessTokenLifetime=1*60*60,
                    RefreshTokenExpiration=TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime= (int) (DateTime.Now.AddDays(60)- DateTime.Now).TotalSeconds,
                    RefreshTokenUsage = TokenUsage.ReUse,

                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                //new IdentityResources.Email(),
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "Identity",  displayName: "View account"),
              //  new ApiScope(name: "Identity.Admin", displayName: "View and edit users"), //?
                new ApiScope(name: "Identity.Offline_access", displayName: "Offline accesss"),
                new ApiScope(name: "IdentityServerApi", displayName: "IdentityServerApi")
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource {
                    Name = "Identity WebApi",
                    DisplayName ="Identity WebApi",
                    Description = "Identity WebApi Access",
                   // UserClaims = new List<string> {"role"},
                    ApiSecrets = new List<Secret> {new Secret("IdentityWebApiSecret".Sha256())},
                    // include the following using claims in access token (in addition to subject id)
                    UserClaims = {
                       // "role",
                        ClaimTypes.Name,
                      //  "UserName",
                        ClaimTypes.Email,
                        ClaimTypes.Role,
                       // "role",
                        //"UserVerified",
                        //"Username",
                        //"user_id"
                     },
                    Scopes = new List<string>{ "Identity", "Identity.Offline_access IdentityServerApi"},
                }
            };
        }

        //public static List<TestUser> TestUsers = new List<TestUser>
        //{

        //    new TestUser
        //    {
        //        SubjectId = "10",
        //        Username = "1",
        //        Password = "Password",
        //        //Claims = new List<Claims>{
        //        //        "role",
        //        //        "UserName",
        //        //        "UserVerified",
        //        //        "Username",
        //        //        "user_id"
        //        //}
        //    },
        //};
    }
}
