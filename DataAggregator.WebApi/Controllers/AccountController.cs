using DataAggregator.WebApi.Models;
using DataAggregator.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataAggregator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService authorizationService;

        public AccountController(IAuthenticationService authorizationService)
        {
            this.authorizationService =
                authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateRequest requestModel)
        {
            var response = await this.authorizationService.AuthenticateAsync(requestModel);

            return Ok(response);
        }
    }
}
