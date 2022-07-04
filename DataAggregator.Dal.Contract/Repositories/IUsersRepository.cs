using DataAggregator.Dal.Contract.Dtos;

namespace DataAggregator.Dal.Contract.Repositories
{
    public interface IUsersRepository
    {
        Task<int> AddUserAsync(UserDto userDto);

        Task<UserDto> GetUserByIdAsync(int userId);

        Task<UserDto> GetUserByEmailAsync(string email);

        IAsyncEnumerable<UserDto> GetUsersAsync();

        Task<bool> DeleteAsync(int userId);

        Task<bool> UpdateUserAsync(int employeeId, UserDto userDto);
    }
}
