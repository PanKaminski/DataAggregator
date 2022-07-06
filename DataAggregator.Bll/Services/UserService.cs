using AutoMapper;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;

namespace DataAggregator.Bll.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IMapper mapper;
        private readonly IApiTasksRepository apiTasksRepository;

        public UserService(IUsersRepository usersRepository, IApiTasksRepository apiTasksRepository, IMapper mapper)
        {
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.apiTasksRepository = apiTasksRepository ?? throw new ArgumentNullException(nameof(apiTasksRepository));
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = this.mapper.Map<User>(await this.usersRepository.GetByIdAsync(id));

            var apiSubscriptions = new List<ApiTask>();

            await foreach (var apiTaskDto in this.apiTasksRepository.GetByUserIdAsync(user.Id))
            {
                apiSubscriptions.Add(this.mapper.Map<ApiTask>(apiTaskDto));
            }
            
            user.ApiSubscriptions = apiSubscriptions;

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = this.mapper.Map<User>(await this.usersRepository.GetByEmailAsync(email));

            var apiSubscriptions = new List<ApiTask>();

            await foreach (var apiTaskDto in this.apiTasksRepository.GetByUserIdAsync(user.Id))
            {
                apiSubscriptions.Add(this.mapper.Map<ApiTask>(apiTaskDto));
            }

            user.ApiSubscriptions = apiSubscriptions;


            return user;
        }

        public async IAsyncEnumerable<User> GetAllAsync()
        {
            await foreach (var userDto in this.usersRepository.GetAllAsync())
            {
                yield return mapper.Map<User>(userDto);
            }
        }

        public async Task<int> AddAsync(User user)
        {
            var count = await this.usersRepository.GetCountAsync();

            if (count == 0)
            {
                user.Role = UserRole.Admin;
            }

            return await this.usersRepository.AddAsync(this.mapper.Map<UserDto>(user));
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            return await this.usersRepository.DeleteAsync(userId);
        }

        public async Task<bool> UpdateAsync(int userId, User user)
        {
            return await this.usersRepository.UpdateAsync(userId, this.mapper.Map<UserDto>(user));
        }
    }
}
