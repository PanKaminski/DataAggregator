using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IDataManager
    {
        Task<bool> ForwardDataAsync(ApiTask apiTask);
    }
}
