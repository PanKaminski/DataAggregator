using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Quartz;

namespace DataAggregator.WebApi.Cron
{
    public class CronJob : IJob
    {
        private readonly IDataManager dataManager;
        private readonly IServiceProvider serviceProvider;

        public CronJob(IDataManager dataManager, IServiceProvider serviceProvider)
        {
            this.dataManager = dataManager;
            this.serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var apiTask = (ApiTask)context.MergedJobDataMap["apiTask"];

            if (await this.dataManager.ForwardDataAsync(apiTask))
            {
                await using var scope = this.serviceProvider.CreateAsyncScope();

                var userService = serviceProvider.GetRequiredService<IUserService>();
                await userService.UpdateStatisticsAsync(apiTask.Subscriber.Id);
            }
        }
    }
}
