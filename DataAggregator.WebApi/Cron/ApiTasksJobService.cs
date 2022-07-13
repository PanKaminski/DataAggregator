using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Quartz;
using Quartz.Spi;

namespace DataAggregator.WebApi.Cron
{
    public class ApiTasksJobService : IApiTasksJobService, IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IJobFactory jobFactory;
        private readonly ILogger logger;

        public ApiTasksJobService(IScheduler scheduler,
            IServiceProvider serviceProvider,
            IJobFactory jobFactory, ILogger<ApiTasksJobService> logger)
        {
            this.Scheduler = scheduler;
            this.serviceProvider = serviceProvider;
            this.jobFactory = jobFactory;
            this.logger = logger;
        }

        private IScheduler Scheduler { get; }

        public async Task AddJobAsync(ApiTask apiTask)
        {
            logger.LogInformation("Schedule job " + apiTask.Name);

            var trigger = TriggerBuilder.Create()
                .WithIdentity(apiTask.Id + ".trigger", apiTask.Subscriber.Email)
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
            logger.LogInformation("Unschedule job " + apiTask.Name);

            Scheduler.UnscheduleJob(new TriggerKey(apiTask.Id.ToString(), apiTask.Subscriber.Email));
            Scheduler.DeleteJob(new JobKey(apiTask.Id.ToString(), apiTask.Subscriber.Email));
        }

        public async Task UpdateJobAsync(ApiTask apiTask)
        {
            logger.LogInformation("Update job " + apiTask.Name);

            this.DeleteJob(apiTask);
            await this.AddJobAsync(apiTask);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
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

            logger.LogInformation("Start scheduler");

            await this.Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stop scheduler");

            await Scheduler?.Shutdown(cancellationToken);
            await Task.CompletedTask;
        }
    }
}
