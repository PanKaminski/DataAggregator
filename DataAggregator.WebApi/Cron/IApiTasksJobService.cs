using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Cron
{
    public interface IApiTasksJobService
    {
        Task AddJobAsync(ApiTask apiTask);

        void DeleteJob(ApiTask apiTask);

        Task UpdateJobAsync(ApiTask apiTask);
    }
}
