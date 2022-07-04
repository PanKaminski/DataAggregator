using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataAggregator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDataManager dataManager;

        public UserController(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        [HttpGet("/")]
        public async Task<ActionResult> Index()
        {
            var subscriptions = new List<ApiTask>();

            var user = new User
            {
                Id = 1,
                Email = "ksendzfs@gmail.com",
                PasswordHash = "",
                ApiSubscriptions = null
            };

            var apiTask = new ApiTask
            {
                Api = new CoinRankingApi
                {
                    ReferenceCurrency = "Euro",
                    Id = 1,
                    Description = "null",
                    SparklineTime = "3h",
                },

                Subscriber = user,
                CronTimeExpression = "*****"
            };

            subscriptions.Add(apiTask);

            user.ApiSubscriptions = subscriptions;

            await dataManager.ForwardDataForUserAsync(user);

            return Ok();
        }
    }
}
