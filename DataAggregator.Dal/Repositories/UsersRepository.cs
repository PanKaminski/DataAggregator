using System.Data;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;
using Microsoft.Data.Sqlite;

namespace DataAggregator.Dal.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly SqliteConnection sqlConnection;

        public UsersRepository(SqliteConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public async Task<int> AddUserAsync(UserDto userDto)
        {
            if (userDto is null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO users (email, role, password_hash, count_of_requests, registration_date) " +
                              "VALUES(@email, @role, @password, @countOfRequests, @registerDate); ",
                Connection = this.sqlConnection,
            };

            AddSqlParameters(userDto, sqlCommand);

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            return await this.SelectUserAsync(userId);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("User email is null or whitespace.", nameof(email));
            }

            return await this.SelectUserAsync(email);
        }

        public async IAsyncEnumerable<UserDto> GetUsersAsync()
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = "SELECT * FROM users"
            };

            await this.sqlConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateUser(reader);
            }

            await this.sqlConnection.CloseAsync();
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = this.sqlConnection,
                CommandText = "DELETE FROM users WHERE id=@userId"
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await this.sqlConnection.OpenAsync();

            var result = await sqlCommand.ExecuteNonQueryAsync() > 0;

            await this.sqlConnection.CloseAsync();

            return result;
        }

        public async Task<bool> UpdateUserAsync(int userId, UserDto userDto)
        {
            if (userDto is null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "UPDATE users " +
                              "SET email = @email," +
                              "role = @role," +
                              "password_hash = @password,",
                Connection = this.sqlConnection,
            };

            AddSqlParameters(userDto, sqlCommand);

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows > 0;
        }

        private async Task<UserDto> SelectUserAsync(string email)
        {
            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM users WHERE email = @email",
                Connection = this.sqlConnection,
            };

            const string emailParameter = "@email";
            sqlCommand.Parameters.Add(emailParameter, SqliteType.Text);
            sqlCommand.Parameters[emailParameter].Value = email;

            await this.sqlConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new KeyNotFoundException("User with such id wasn't found.");
            }

            await reader.ReadAsync();

            var user = CreateUser(reader);

            await this.sqlConnection.CloseAsync();

            return user;
        }

        private async Task<UserDto> SelectUserAsync(int userId)
        {
            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM users WHERE id = @userId",
                Connection = this.sqlConnection,
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await this.sqlConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new KeyNotFoundException("User with such id wasn't found.");
            }

            await reader.ReadAsync();

            var user = CreateUser(reader);

            await this.sqlConnection.CloseAsync();

            return user;
        }

        private static UserDto CreateUser(SqliteDataReader reader)
        {
            if (!Enum.TryParse((string)reader["role"], out UserRoleDto role))
            {
                role = UserRoleDto.User;
            }

            return new UserDto
            {
                Id = (int)reader["id"],
                Email = (string)reader["email"],
                Role = role,
                PasswordHash = (string)reader["password_hash"],
                CountOfRequests = (int)reader["count_of_requests"],
                RegistrationDate = DateTime.Parse((string)reader["registration_date"]),
                ApiSubscriptions = new List<ApiTaskDto>(),
            };
        }

        private static void AddSqlParameters(UserDto user, SqliteCommand command)
        {
            const string emailParameter = "@email";
            command.Parameters.Add(emailParameter, SqliteType.Text);
            command.Parameters[emailParameter].Value = user.Email;

            const string roleParameter = "@role";
            command.Parameters.Add(roleParameter, SqliteType.Text);
            command.Parameters[roleParameter].Value = user.Role.ToString();

            const string passwordParameter = "@password";
            command.Parameters.Add(passwordParameter, SqliteType.Text);
            command.Parameters[passwordParameter].Value = user.PasswordHash;
        }
    }
}
