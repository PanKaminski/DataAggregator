using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Quartz;
using Quartz.Spi;

namespace DataAggregator.WebApi.Cron
{
    public class ApiTasksJobService : IApiTasksJobService, IHostedService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly IJobFactory jobFactory;

        private IScheduler Scheduler { get; set; }

        public ApiTasksJobService(ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider, IJobFactory jobFactory)
        {
            this.schedulerFactory = schedulerFactory;
            this.serviceProvider = serviceProvider;
            this.jobFactory = jobFactory;
        }

        public async Task AddJobAsync(ApiTask apiTask)
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity(apiTask.Id.ToString(), apiTask.Subscriber.Email)
                .WithCronSchedule(apiTask.CronTimeExpression)
                .StartNow()
                .Build();

            var jobDetails = JobBuilder.Create<CronJob>()
                .WithIdentity(apiTask.Id.ToString(), apiTask.Subscriber.Email)
                .Build();

            jobDetails.JobDataMap.Put("apiTask", apiTask);

            await this.Scheduler.ScheduleJob(jobDetails, trigger);
        }

        public void DeleteJob(ApiTask apiTask)
        {
            Scheduler.UnscheduleJob(new TriggerKey(apiTask.Id.ToString(), apiTask.Subscriber.Email));
            Scheduler.DeleteJob(new JobKey(apiTask.Id.ToString(), apiTask.Subscriber.Email));
        }

        public async Task UpdateJobAsync(ApiTask apiTask)
        {
            this.DeleteJob(apiTask);
            await this.AddJobAsync(apiTask);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            this.Scheduler.JobFactory = this.jobFactory;

            if (!cancellationToken.IsCancellationRequested)
            {
                await using var scope = this.serviceProvider.CreateAsyncScope();

                var apiTasksService = scope.ServiceProvider.GetRequiredService<IApiTasksService>();

                await foreach (var task in apiTasksService.GetAllAsync())
                {
                    await this.AddJobAsync(task);
                }
            }

            await this.Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
            await Task.CompletedTask;
        }
    }
}
