using AutoMapper;
using Identity.Application.Dtos;
using Identity.Domain.ErrorModels;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class IdentityTokenService: ITokenService
    {
        private readonly IMapper _mapper;
        private readonly string _url;
        private readonly ILogger<IdentityTokenService> _logger;

        public IdentityTokenService(IMapper mapper, IConfiguration configuration,
            ILogger<IdentityTokenService> logger)
        {
            _mapper = mapper;
            _url = configuration["ID4:Authority"];
            _logger = logger;
        }

        public async Task<Result<TokenViewModel>> GetTokenAsync(LoginUser loginUser)
        {
            _logger.LogInformation("Getting access token");

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _url,
                Policy = new DiscoveryPolicy { RequireHttps = false, }
            });

            if (disco.IsError)
            {
                _logger.LogError(disco.Error, "Error getting discovery document");
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
            _logger.LogInformation("Access token retrieved successfully");

            return new Result<TokenViewModel>() { Value = tokenRes };
        }

        public async Task<TokenViewModel> GetRefreshedTokenPairAsync(string clientId, string clientSecret, string refreshToken)
        {
            _logger.LogInformation("Refreshing token pair");
            using var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _url,
                Policy = new DiscoveryPolicy { RequireHttps = false, }
            });

            if (disco.IsError)
            {
                _logger.LogError(disco.Error, "Error getting discovery document");
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
                _logger.LogError("Error refreshing token pair");
                return null;
            }

            var tokenRes = _mapper.Map<TokenViewModel>(refreshTokenResponse);
            _logger.LogInformation("Token pair refreshed successfully");

            return tokenRes;
        }
    }
}
