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
            if (apiTaskId <= 0)
            {
                return this.BadRequest();
            }

            var model = await this.apiTasksService.GetAsync(apiTaskId);
            var user = (User)this.HttpContext.Items["User"];

            if (user is null || model.Subscriber.Id != user.Id)
            {
                return this.BadRequest();
            }

            return Ok(this.mapper.Map<ApiTaskResponse>(model));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync(ApiTaskCreateRequest taskViewModel)
        {
            if (taskViewModel is null)
            {
                return this.BadRequest();
            }

            var model = this.mapper.Map<ApiTask>(taskViewModel);
            model.Subscriber = (User)this.HttpContext.Items["User"];

            var id = await this.apiTasksService.AddAsync(model);

            return id > 0 ? this.Ok(id) : this.BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, ApiTaskCreateRequest taskViewModel)
        {
            if (taskViewModel is null)
            {
                return this.BadRequest();
            }

            var currentData = await this.apiTasksService.GetAsync(id);
            var user = (User)this.HttpContext.Items["User"];

            if (currentData is null || currentData.Subscriber.Id != user.Id)
            {
                return this.BadRequest();
            }

            var model = this.mapper.Map<ApiTask>(taskViewModel);
            model.Subscriber = user;

            var isUpdated = await this.apiTasksService.UpdateAsync(id, model);

            return isUpdated ? this.NoContent() : this.BadRequest();
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
