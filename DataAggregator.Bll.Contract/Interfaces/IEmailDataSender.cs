using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IEmailDataSender
    {
        Task SendDataOnEmailAsync(MessageDetails messageDetails);
    }
}
