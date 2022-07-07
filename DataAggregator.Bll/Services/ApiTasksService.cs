using AutoMapper;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;

namespace DataAggregator.Bll.Services
{
    public class ApiTasksService : IApiTasksService
    {
        private readonly IApiTasksRepository apiTasksRepository;
        private readonly IMapper mapper;

        public ApiTasksService(IApiTasksRepository apiTasksRepository, IMapper mapper)
        {
            this.apiTasksRepository = apiTasksRepository ?? throw new ArgumentNullException(nameof(apiTasksRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async IAsyncEnumerable<ApiTask> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                yield break;
            }

            var tasks = apiTasksRepository.GetByUserIdAsync(userId);

            await foreach (var taskDto in tasks)
            {
                yield return this.mapper.Map<ApiTask>(taskDto);
            }
        }

        public async IAsyncEnumerable<ApiTask> GetAllAsync()
        {
            var tasks = apiTasksRepository.GetAsync();

            await foreach (var taskDto in tasks)
            {
                yield return this.mapper.Map<ApiTask>(taskDto);
            }
        }

        public async Task<ApiTask> GetAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Api task id must be positive.");
            }

            return this.mapper.Map<ApiTask>(await this.apiTasksRepository.GetAsync(apiTaskId));
        }

        public async Task<int> AddAsync(ApiTask apiTask)
        {
            if (apiTask is null)
            {
                return -1;
            }

            return await this.apiTasksRepository.AddAsync(this.mapper.Map<ApiTaskDto>(apiTask));
        }

        public async Task<bool> DeleteAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                return false;
            }

            return await this.apiTasksRepository.DeleteAsync(apiTaskId);
        }

        public async Task<bool> UpdateAsync(int apiTaskId, ApiTask apiTask)
        {
            if (apiTask is null || apiTaskId <= 0)
            {
                return false;
            }

            return await this.apiTasksRepository.UpdateAsync(apiTaskId, this.mapper.Map<ApiTaskDto>(apiTask));
        }
    }
}
