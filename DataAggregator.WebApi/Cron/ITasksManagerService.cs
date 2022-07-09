using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Cron
{
    public interface ITasksManagerService
    {
        Queue<ApiTask> AggregatorTasks { get; set; }
    }
}
