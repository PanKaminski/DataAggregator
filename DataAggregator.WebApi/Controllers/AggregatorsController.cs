﻿using AutoMapper;
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

        [HttpGet]
        public async IAsyncEnumerable<ApiTaskItemResponse> Index()
        {
            var user = (User)this.HttpContext.Items["User"];

            await foreach (var apiTask in this.apiTasksService.GetByUserIdAsync(user.Id))
            {
                yield return this.mapper.Map<ApiTaskItemResponse>(apiTask);
            }
        }

        [HttpGet("{apiTaskId}")]
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

            var response = this.mapper.Map<ApiTaskResponse>(model);

            return Ok(response);
        }

        [HttpPost("weather")]
        public async Task<IActionResult> CreateApiTaskWithWeatherTrackerAsync(WeatherApiTaskRequest taskViewModel)
        {
            var id = await this.CreateApiTask(taskViewModel);

            return id > 0 ? this.Ok(id) : this.BadRequest();
        }

        [HttpPost("coin")]
        public async Task<IActionResult> CreateApiTaskWithCoinTrackerAsync(CoinRankingApiTaskRequest taskViewModel)
        {
            var id = await this.CreateApiTask(taskViewModel);

            return id > 0 ? this.Ok(id) : this.BadRequest();
        }

        [HttpPost("covid")]
        public async Task<IActionResult> CreateApiTaskWithCovidTrackerAsync(CovidApiTaskRequest taskViewModel)
        {
            var id = await this.CreateApiTask(taskViewModel);

            return id > 0 ? this.Ok(id) : this.BadRequest();
        }

        [HttpPut("weather/{id}")]
        public async Task<IActionResult> UpdateWeatherTaskAsync(int id, WeatherApiTaskRequest taskViewModel)
        {
            var updateResult = await UpdateApiTask(id, taskViewModel);

            return updateResult.Item1 ? this.NoContent() : this.BadRequest(updateResult.Item2);
        }

        [HttpPut("covid/{id}")]
        public async Task<IActionResult> UpdateCovidTaskAsync(int id, CovidApiTaskRequest taskViewModel)
        {
            var updateResult = await UpdateApiTask(id, taskViewModel);

            return updateResult.Item1 ? this.NoContent() : this.BadRequest(updateResult.Item2);
        }

        [HttpPut("coin/{id}")]
        public async Task<IActionResult> UpdateCoinRankingTaskAsync(int id, CoinRankingApiTaskRequest taskViewModel)
        {
            var updateResult = await UpdateApiTask(id, taskViewModel);

            return updateResult.Item1 ? this.NoContent() : this.BadRequest(updateResult.Item2);
        }


        [HttpDelete("{apiTaskId}")]
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

        private async Task<int> CreateApiTask(ApiTaskCreateRequest apiTaskModel)
        {
            if (apiTaskModel is null)
            {
                return -1;
            }

            var model = this.mapper.Map<ApiTask>(apiTaskModel);
            model.Subscriber = (User)this.HttpContext.Items["User"];

            var id = await this.apiTasksService.AddAsync(model);

            return id;
        }

        private async Task<(bool, string)> UpdateApiTask(int id, ApiTaskCreateRequest taskViewModel)
        {
            if (taskViewModel is null)
            {
                return (false, "Invalid values for api task");
            }

            var currentData = await this.apiTasksService.GetAsync(id);
            var user = (User)this.HttpContext.Items["User"];

            if (currentData is null || currentData.Subscriber.Id != user.Id)
            {
                return (false, "Current user is not able to modify task.");
            }

            var model = this.mapper.Map<ApiTask>(taskViewModel);
            model.Subscriber = user;

            var isUpdated = await this.apiTasksService.UpdateAsync(id, model);

            return (isUpdated, string.Empty);
        }
    }
}
