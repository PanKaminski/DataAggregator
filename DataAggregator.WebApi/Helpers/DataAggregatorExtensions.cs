using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Infrastructure;
using DataAggregator.Bll.Services;
using DataAggregator.Dal.Contract.Repositories;
using DataAggregator.Dal.Repositories;
using DataAggregator.WebApi.Cron;
using Quartz.Impl;
using Quartz.Spi;

namespace DataAggregator.WebApi.Helpers
{
    public static class DataAggregatorExtensions
    {
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailCredentials = configuration.GetSection("EmailCredentials").Get<EmailCredentials>();
            services.AddSingleton(emailCredentials);
            services.AddHttpClient<IDataAggregator, TaskDataAggregator>();
            services.AddSingleton<IEmailDataSender, EmailDataSender>();
            services.AddSingleton<IDataManager, DataManager>();

            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);

            services.AddSingleton<IJobFactory, CronJobFactory>();
            services.AddSingleton<IApiTasksJobService, ApiTasksJobService>();
            services.AddHostedService<ApiTasksJobService>();

            return services;
        }

        public static IServiceCollection AddBllServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApiTasksService, ApiTasksService>();

            services.AddScoped<IUsersRepository>(_ => new UsersRepository(connectionString));
            services.AddScoped<IApiTasksRepository>(_ => new ApiTasksRepository(connectionString));

            return services;
        }
    }
}
