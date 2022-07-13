using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using Microsoft.Extensions.Logging;

namespace DataAggregator.Bll.Services
{
    public class DataManager : IDataManager
    {
        private readonly IDataAggregator aggregator;
        private readonly IEmailDataSender emailDataSender;
        private readonly ILogger logger;

        public DataManager(IDataAggregator aggregator, IEmailDataSender emailDataSender, ILogger<DataManager> logger)
        {
            this.aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            this.emailDataSender = emailDataSender ?? throw new ArgumentNullException(nameof(emailDataSender));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ForwardDataAsync(ApiTask apiTask)
        {
            if (apiTask is null)
            {
                logger.LogError("Api aggregator job failed. Api task is null.");
                return false;
            }

            try
            {
                var data = await aggregator.ReceiveDataAsync(apiTask.Api);

                if (string.IsNullOrWhiteSpace(data))
                {
                    logger.LogInformation("Empty data received.");

                    return false;
                }

                using var stream = new MemoryStream();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(data);
                await writer.FlushAsync();
                stream.Position = 0;

                await emailDataSender.SendDataOnEmailAsync(
                    new MessageDetails(apiTask.Subscriber.Email, apiTask.Subscriber.Email, "Aggregation data", stream));

                return true;
            }
            catch (ArgumentNullException exception)
            {
                logger.LogError($"Parameter is null - {exception.ParamName}", exception);
                return false;
            }
            catch (Exception exception)
            {
                logger.LogError($"Failed data forward to email { apiTask.Subscriber?.Email } - {exception.Message}", exception);
                return false;
            }
        }
    }
}
