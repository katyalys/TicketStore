using AutoMapper;
using Identity.Application.Dtos;
using IdentityModel.Client;

namespace IdentityServer
{
    public class IdentityTokenService: ITokenService
    {
        private readonly IMapper _mapper;

        public IdentityTokenService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<TokenViewModel> GetToken(LoginUser loginUser)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://localhost:5012",
                Policy = new DiscoveryPolicy { RequireHttps = false, }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = "identityUserClient",
                ClientSecret = "IdentityApiSecret",
                UserName = loginUser.UserName,
                Password = loginUser.Password,
                Address = disco.TokenEndpoint,
                Scope = "Identity offline_access IdentityServerApi",
            };

            var token = await client.RequestPasswordTokenAsync(passwordTokenRequest);

            var tokenRes = _mapper.Map<TokenViewModel>(token);
            return tokenRes;
        }

        public async Task<TokenViewModel> GetRefreshedTokenPairAsync(string clientId, string clientSecret, string refreshToken)
        {
            using var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://localhost:5012",
                Policy = new DiscoveryPolicy { RequireHttps = false, }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshTokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                RefreshToken = refreshToken
            });

            if (refreshTokenResponse.IsError)
            {
                return null;
            }

            var tokenRes = _mapper.Map<TokenViewModel>(refreshTokenResponse);
            return tokenRes;
        }
    }
}
