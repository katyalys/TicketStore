using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;


namespace Identity.WebApi
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserManager<IdentityUser> _userManager;
        public ResourceOwnerPasswordValidator(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db (by username - in my case its email)
               // var user1 =  _userManager.Users.ToList();
                var passwordHasher = new PasswordHasher<IdentityUser>();
                var user = await _userManager.FindByNameAsync(context.UserName);
                if (user != null)
                {
                    var passwordHash = passwordHasher.HashPassword(user, context.Password);
                    //check if password match - remember to hash password if stored as hash in db
                    if (await _userManager.CheckPasswordAsync(user, context.Password))
                    { 
                        var s = await _userManager.GetClaimsAsync(user);
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.Id.ToString(),
                            authenticationMethod: "custom",
                            claims: await _userManager.GetClaimsAsync(user));
                        return;
                    }
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                return;
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
                Console.WriteLine(ex);
            }
        }
    }
}
