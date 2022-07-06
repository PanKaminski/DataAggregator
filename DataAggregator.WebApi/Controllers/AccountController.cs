using AutoMapper;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Models;
using DataAggregator.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataAggregator.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService authorizationService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AccountController(IAuthenticationService authorizationService, IUserService userService, IMapper mapper)
        {
            this.authorizationService =
                authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateRequest requestModel)
        {
            var response = await this.authorizationService.AuthenticateAsync(requestModel);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest requestModel)
        {
            var result = await this.userService.AddAsync(new User
            {
                Id = 0,
                Email = requestModel.Email,
                Role = UserRole.User,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestModel.Password),
                CountOfRequests = 0,
            });

            if (result < 1)
            {
                return this.BadRequest();
            }

            var response = await this.authorizationService.AuthenticateAsync(requestModel);

            return Ok(response);
        }

        [HttpGet("statistics")]
        [Authorize(UserRole.Admin)]
        public async IAsyncEnumerable<StatisticsResponse> ViewAll()
        {
            await foreach (var user in this.userService.GetAllAsync())
            {
                yield return this.mapper.Map<StatisticsResponse>(user);
            }
        }
    }
}
