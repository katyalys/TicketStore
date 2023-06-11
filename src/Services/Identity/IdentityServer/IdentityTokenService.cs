using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Dtos;
using Identity.Application.Services;
using Identity.Domain.ErrorModels;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public class IdentityTokenService: ITokenService
    {

        private readonly IMapper _mapper;
        private IValidator<LoginUser> _validator;
        private readonly string _url;

        public IdentityTokenService(IMapper mapper, IValidator<LoginUser> validator, IConfiguration configuration)
        {
            _mapper = mapper;
            _validator = validator;
            _url = configuration["ID4:Authority"];
        }

        public async Task<Result<TokenViewModel>> GetToken(LoginUser loginUser)
        {
            ValidationResult result = await _validator.ValidateAsync(loginUser);
            if (!result.IsValid)
            {
                return ResultReturnService.CreateErrorResult<TokenViewModel>(ErrorStatusCode.WrongAction, "Invalid values");
            }

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _url,
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

            return new Result<TokenViewModel>() { Value = tokenRes };
        }

        public async Task<TokenViewModel> GetRefreshedTokenPairAsync(string clientId, string clientSecret, string refreshToken)
        {
            using var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _url,
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
