using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.WebApi.Authorization;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Models;
using Microsoft.Extensions.Options;

namespace DataAggregator.WebApi.Services;

public class AuthenticationService : IAuthenticationService
{
    private IUserService userService;
    private IJwtUtils jwtUtils;
    private readonly AppSettings appSettings;

    public AuthenticationService(IUserService userService, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
    {
        this.userService = userService;
        this.jwtUtils = jwtUtils;
        this.appSettings = appSettings.Value;
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest requestModel)
    {
        var user = await this.userService.GetByEmailAsync(requestModel.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(requestModel.Password, user.PasswordHash))
        {
            throw new AppException("Username or password is incorrect");
        }

        var jwtToken = jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, jwtToken);
    }
}