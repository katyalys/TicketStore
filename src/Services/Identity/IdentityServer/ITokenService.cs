﻿using Identity.Application.Dtos;
using Identity.Domain.ErrorModels;

namespace IdentityServer
{
    public interface ITokenService
    {
        Task<Result<TokenViewModel>> GetTokenAsync(LoginUser loginUser);
        Task<TokenViewModel> GetRefreshedTokenPairAsync(string clientId, string clientSecret, string refreshToken);
    }
}
