﻿using Duende.IdentityServer;
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
        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
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
                    AllowedScopes = new List<string> { "offline_access", "Identity", "IdentityServerApi"},
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
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "Identity",  displayName: "View account"),
                new ApiScope(name: "offline_access", displayName: "Offline accesss"),
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
                    ApiSecrets = new List<Secret> {new Secret("IdentityWebApiSecret".Sha256())},
                    // include the following using claims in access token (in addition to subject id)
                    UserClaims = {
                        "role",
                     },
                    Scopes = new List<string>{ "Identity", "offline_access", "IdentityServerApi"},
                }
            };
        }
    }
}
