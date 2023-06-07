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
            //await _userManager.SetLockoutEnabledAsync(user, false);
            var isDeleted = await _userManager.GetLockoutEnabledAsync(user);
            if (isDeleted == true)
                throw new Exception("User is deleted");

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
