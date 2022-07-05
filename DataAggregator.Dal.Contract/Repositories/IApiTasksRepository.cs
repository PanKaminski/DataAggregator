using DataAggregator.Dal.Contract.Dtos;

namespace DataAggregator.Dal.Contract.Repositories
{
    public interface IApiTasksRepository
    {
        Task<int> AddAsync(ApiTaskDto apiTask);

        IAsyncEnumerable<ApiTaskDto> GetByUserIdAsync(int userId);

        IAsyncEnumerable<ApiTaskDto> GetAsync();

        Task<bool> DeleteAsync(int apiTaskId);

        Task<bool> UpdateAsync(int apiTaskId, ApiTaskDto apiTask);
    }
}
