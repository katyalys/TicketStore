using Identity.Application.Dtos;

namespace IdentityServer
{
    public interface ITokenService
    {
        Task<TokenViewModel> GetToken(LoginUser loginUser);
        Task<TokenViewModel> GetRefreshedTokenPairAsync(string clientId, string clientSecret, string refreshToken);

    }
}
