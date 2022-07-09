using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Cron
{
    public class TasksManagerService : ITasksManagerService
    {
        public Queue<ApiTask> AggregatorTasks { get; set; }
    }
}
