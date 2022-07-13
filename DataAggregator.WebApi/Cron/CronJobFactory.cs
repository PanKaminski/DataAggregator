using Quartz;
using Quartz.Spi;

namespace DataAggregator.WebApi.Cron
{
    public class CronJobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CronJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            return (IJob)this.serviceProvider.GetService(jobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
