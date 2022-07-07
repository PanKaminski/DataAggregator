using AutoMapper;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.WebApi.Helpers;
using DataAggregator.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataAggregator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AggregatorsController : ControllerBase
    {
        private readonly IApiTasksService apiTasksService;
        private readonly IMapper mapper;

        public AggregatorsController(IApiTasksService apiTasksService, IMapper mapper)
        {
            this.apiTasksService = apiTasksService ?? throw new ArgumentNullException(nameof(apiTasksService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("/")]
        public async IAsyncEnumerable<ApiTaskItemResponse> Index()
        {
            var user = (User)this.HttpContext.Items["User"];

            await foreach (var apiTask in this.apiTasksService.GetByUserIdAsync(user.Id))
            {
                yield return this.mapper.Map<ApiTaskItemResponse>(apiTask);
            }
        }

        [HttpGet("/{apiTaskId}")]
        public async Task<IActionResult> GetByIdAsync(int apiTaskId)
        {
            return Ok();
        }

        [HttpDelete("/{apiTaskId}")]
        public async Task<IActionResult> DeleteAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                return BadRequest("Invalid api task id");
            }

            var isDeleted = await this.apiTasksService.DeleteAsync(apiTaskId);

            if (!isDeleted)
            {
                return NotFound();
            }

            return this.NoContent();
        }
    }
}
