using Cronos;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Timer = System.Timers.Timer;

namespace DataAggregator.WebApi.Cron
{
    public class ApiAggregatorHostedService : IHostedService, IDisposable
    {
        private System.Timers.Timer timer;
        private readonly TimeZoneInfo timeZoneInfo;
        private readonly ITasksManagerService taskManager;
        private readonly IApiTasksService apiTasksService;
        private readonly IDataManager dataProcessor;

        public ApiAggregatorHostedService(Timer timer, TimeZoneInfo timeZoneInfo, 
            ITasksManagerService taskManager, IApiTasksService apiTasksService, IDataManager dataProcessor)
        {
            this.timer = timer;
            this.timeZoneInfo = timeZoneInfo;
            this.taskManager = taskManager;
            this.apiTasksService = apiTasksService;
            this.dataProcessor = dataProcessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await foreach (var task in apiTasksService.GetAllAsync())
            {
                this.taskManager.AggregatorTasks.Enqueue(task);
            }

            while (this.taskManager.AggregatorTasks.Count != 0)
            {
                var apiTask = this.taskManager.AggregatorTasks.Dequeue();
                await ScheduleJobAsync(apiTask, cancellationToken);
            }
        }

        protected virtual async Task ScheduleJobAsync(ApiTask apiTask, CancellationToken cancellationToken)
        {
            var expression = CronExpression.Parse(apiTask.CronTimeExpression);

            var next = expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);

            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;

                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJobAsync(apiTask, cancellationToken);
                }

                timer = new Timer(delay.TotalMilliseconds);

                timer.Elapsed += async (sender, args) =>
                {
                    timer.Dispose();  // reset and dispose timer
                    timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await MakeForwardJobAsync(apiTask, cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJobAsync(apiTask, cancellationToken);    // reschedule next
                    }
                };

                timer.Start();
            }

            await Task.CompletedTask;
        }

        public virtual async Task MakeForwardJobAsync(ApiTask apiTask, CancellationToken cancellationToken)
        {
            await this.dataProcessor.ForwardDataAsync(apiTask);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Stop();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
