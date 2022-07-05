using DataAggregator.Dal.Contract.Dtos;

namespace DataAggregator.Dal.Contract.Repositories
{
    public interface IUsersRepository
    {
        Task<int> AddAsync(UserDto userDto);

        Task<UserDto> GetByIdAsync(int userId);

        Task<UserDto> GetByEmailAsync(string email);

        IAsyncEnumerable<UserDto> GetAllAsync();

        Task<bool> DeleteAsync(int userId);

        Task<bool> UpdateAsync(int employeeId, UserDto userDto);
    }
}
