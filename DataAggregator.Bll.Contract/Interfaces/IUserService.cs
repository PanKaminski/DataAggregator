using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);

        Task<User> GetByEmailAsync(string email);

        IAsyncEnumerable<User> GetAllAsync();

        Task<int> AddAsync(User user);

        Task<bool> DeleteAsync(int userId);

        Task<bool> UpdateAsync(int userId, User user);
    }
}
