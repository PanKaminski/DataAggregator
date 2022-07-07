using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IApiTasksService
    {
        IAsyncEnumerable<ApiTask> GetByUserIdAsync(int userId);

        IAsyncEnumerable<ApiTask> GetAllAsync();

        Task<ApiTask> GetAsync(int apiTaskId);

        Task<int> AddAsync(ApiTask apiTask);

        Task<bool> DeleteAsync(int apiTaskId);

        Task<bool> UpdateAsync(int apiTaskId, ApiTask apiTask);
    }
}
