using DataAggregator.Dal.Contract.Dtos;

namespace DataAggregator.Dal.Contract.Repositories
{
    internal interface IApiTasksRepository
    {
        Task<int> AddAsync(ApiTaskDto apiTask);

        Task<ApiTaskDto> GetByUserIdAsync(int userId);

        IAsyncEnumerable<ApiTaskDto> GetAsync();

        Task<bool> DeleteAsync(int userId);

        Task<bool> UpdateAsync(int apiTaskId, ApiTaskDto apiTask);
    }
}
