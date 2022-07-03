using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Bll.Contract.Models;
namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);

        Task<User> GetByEmailAsync(string email);

        IAsyncEnumerable<User> GetAllAsync();

        Task<int> Add(User user);

        Task<bool> Delete(int userId);

        Task<bool> Update(User user);
    }
}
