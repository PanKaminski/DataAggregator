using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IDataAggregator
    {
        Task<string> ReceiveDataAsync(AggregatorApi task);
    }
}
