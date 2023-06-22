using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer
{
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

            var isDeleted = await _userManager.GetLockoutEnabledAsync(user);
            if (isDeleted)
            {
                context.IsActive = false;
            }
            else
            {
                context.IsActive = (user != null);
            }
        }
    }
}
