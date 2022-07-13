using System.Data;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;
using Microsoft.Data.Sqlite;

namespace DataAggregator.Dal.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string connectionString;

        public UsersRepository(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> AddAsync(UserDto userDto)
        {
            if (userDto is null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO users (email, role, password_hash, count_of_requests, registration_date) " +
                              "VALUES(@email, @role, @password, @countOfRequests, @registerDate); ",
                Connection = dbConnection,
            };

            AddSqlParametersForInsertion(userDto, sqlCommand);

            await dbConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();

            return affectedRows;
        }

        public async Task<UserDto> GetByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            return await this.SelectUserAsync(userId);
        }

        public async Task<UserDto> GetBySubscriptionAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Task id must be greater than zero.", nameof(apiTaskId));
            }

            return await this.SelectUserByTaskAsync(apiTaskId);
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("User email is null or whitespace.", nameof(email));
            }

            return await this.SelectUserAsync(email);
        }

        public async Task<long> GetCountAsync()
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT COUNT(*) AS count FROM users",
                Connection = dbConnection,
            };

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            await reader.ReadAsync();

            return (long)reader["count"];
        }

        public async IAsyncEnumerable<UserDto> GetAllAsync()
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
                CommandText = "SELECT * FROM users"
            };

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateUser(reader, "id");
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = dbConnection,
                CommandText = "DELETE FROM users WHERE id=@userId"
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int userId, UserDto userDto)
        {
            if (userDto is null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "UPDATE users " +
                              "SET email = @email," +
                              "role = @role," +
                              "password_hash = @password," +
                              "count_of_requests = @countOfRequests," +
                              "registration_date = @registerDate" +
                              "WHERE id = @userId",
                Connection = dbConnection,
            };

            AddSqlParameters(userDto, sqlCommand);

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateStatisticsAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "UPDATE users " +
                              "SET count_of_requests = count_of_requests + 1" +
                              "WHERE id = @userId",
                Connection = dbConnection,
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        private async Task<UserDto> SelectUserAsync(string email)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM users WHERE email = @email",
                Connection = dbConnection,
            };

            const string emailParameter = "@email";
            sqlCommand.Parameters.Add(emailParameter, SqliteType.Text);
            sqlCommand.Parameters[emailParameter].Value = email;

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new KeyNotFoundException("User with such id wasn't found.");
            }

            await reader.ReadAsync();

            return CreateUser(reader, "id");
        }

        private async Task<UserDto> SelectUserAsync(int userId)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM users WHERE id = @userId",
                Connection = dbConnection,
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new KeyNotFoundException("User with such id wasn't found.");
            }

            await reader.ReadAsync();

            return CreateUser(reader, "id");
        }

        private async Task<UserDto> SelectUserByTaskAsync(int apiTaskId)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM api_tasks at JOIN users u on at.subscriber_id = u.id WHERE at.id = @apiTaskId",
                Connection = dbConnection,
            };

            const string apiTaskIdParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTaskId;

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            await reader.ReadAsync();

            return CreateUser(reader, "subscriber_id");
        }

        private static UserDto CreateUser(SqliteDataReader reader, string idColumnName)
        {
            if (!Enum.TryParse((string)reader["role"], out UserRoleDto role))
            {
                role = UserRoleDto.User;
            }

            return new UserDto
            {
                Id = (int)(long)reader[idColumnName],
                Email = (string)reader["email"],
                Role = role,
                PasswordHash = (string)reader["password_hash"],
                CountOfRequests = (int)(long)reader["count_of_requests"],
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

        private static void AddSqlParametersForInsertion(UserDto user, SqliteCommand command)
        {
            AddSqlParameters(user, command);

            const string statisticsParameter = "@countOfRequests";
            command.Parameters.Add(statisticsParameter, SqliteType.Integer);
            command.Parameters[statisticsParameter].Value = user.CountOfRequests;

            const string registerDateParameter = "@registerDate";
            command.Parameters.Add(registerDateParameter, SqliteType.Text);
            command.Parameters[registerDateParameter].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
