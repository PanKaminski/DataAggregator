﻿using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Services
{
    public class DataManager : IDataManager
    {
        private readonly IDataAggregator aggregator;
        private readonly IEmailDataSender emailDataSender;

        public DataManager(IDataAggregator aggregator, IEmailDataSender emailDataSender)
        {
            this.aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            this.emailDataSender = emailDataSender ?? throw new ArgumentNullException(nameof(emailDataSender));
        }

        public async Task ForwardDataForUserAsync(User user)
        {
            var tasks = user.ApiSubscriptions;

            foreach (var task in tasks)
            {
                var data = await aggregator.ReceiveDataAsync(task.Api);

                using var stream = new MemoryStream();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(data);
                await writer.FlushAsync();
                stream.Position = 0;

                await emailDataSender.SendDataOnEmailAsync(
                    new MessageDetails(user.Email, user.Email, "Aggregation data", stream));
            }
        }
    }
}
