using DataAggregator.WebApi.Models;

namespace DataAggregator.WebApi.Services
{
    public interface IAuthenticationService
    {
        public Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest requestModel);
    }
}
