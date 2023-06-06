using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Identity.Domain;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Identity.Domain.Enums.EnumAttributes;

namespace IdentityServer
{
    //public class IdentityProfileService : IProfileService
    //{
    //    private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
    //    private readonly UserManager<User> _userManager;

    //    public IdentityProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsFactory)
    //    {
    //        _userManager = userManager;
    //        _claimsFactory = claimsFactory;
    //    }

    //    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    //    {
    //        var sub = context.Subject.GetSubjectId();
    //        var user = await _userManager.FindByIdAsync(sub);
    //        var principal = await _claimsFactory.CreateAsync(user);

    //        var claims = principal.Claims.ToList();
    //        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
    //        claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
    //        claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

    //        context.IssuedClaims = claims;
    //    }
    //    public async Task IsActiveAsync(IsActiveContext context)
    //    {
    //        var sub = context.Subject.GetSubjectId();
    //        var user = await _userManager.FindByIdAsync(sub);
    //        context.IsActive = user != null;
    //    }
    //}

    public class IdentityProfileService : IProfileService
    {
        private UserManager<IdentityUser> _userManager;
        public IdentityProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = roles.Select(role => new Claim("role", role));

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null);
        }
    }
}
