using System.Data;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;
using Microsoft.Data.Sqlite;

namespace DataAggregator.Dal.Repositories
{
    public class ApiTasksRepository : IApiTasksRepository
    {
        private readonly SqliteConnection sqlConnection;

        public ApiTasksRepository(SqliteConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public async Task<int> AddAsync(ApiTaskDto apiTask)
        {
            if (apiTask is null)
            {
                throw new ArgumentNullException(nameof(apiTask));
            }

            var result = await AddApiTaskAsync(apiTask);
            await AddApiTaskAggregatorAsync(apiTask.Api);
            await AddConcreteApiTaskAggregatorAsync(apiTask.Api);

            return result;
        }

        public async IAsyncEnumerable<ApiTaskDto> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = "SELECT * FROM api_tasks WHERE subscriber_id = @userId"
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await this.sqlConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return await CreateApiTaskAsync(reader);
            }

            await this.sqlConnection.CloseAsync();
        }

        public async IAsyncEnumerable<ApiTaskDto> GetAsync()
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = "SELECT * FROM api_tasks"
            };

            await this.sqlConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return await CreateApiTaskAsync(reader);
            }

            await this.sqlConnection.CloseAsync();
        }

        public async Task<bool> DeleteAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Api task id must be greater than zero.", nameof(apiTaskId));
            }

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = "DELETE * FROM api_tasks WHERE id=@apiTaskId"
            };

            const string apiTaskIdParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTaskId;

            await this.sqlConnection.OpenAsync();

            await this.sqlConnection.OpenAsync();

            var result = await sqlCommand.ExecuteNonQueryAsync() > 0;

            await this.sqlConnection.CloseAsync();

            return result;
        }

        public async Task<bool> UpdateAsync(int apiTaskId, ApiTaskDto apiTask)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Api task id must be greater than zero.", nameof(apiTaskId));
            }

            if (apiTask is null)
            {
                throw new ArgumentNullException(nameof(apiTask));
            }

            var result = await UpdateApiTaskAsync(apiTask) >= 0;

            await this.sqlConnection.CloseAsync();

            return result;
        }

        private async Task<ApiTaskDto> CreateApiTaskAsync(SqliteDataReader reader)
        {
            var apiTask = ReadApiTaskFields(reader);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
                CommandText = "SELECT * FROM apis_aggregators WHERE api_task_key = @apiTaskId"
            };

            const string apiTaskIdParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTask.Id;

            reader = await sqlCommand.ExecuteReaderAsync();

            var api = await ReadAggregationApi(reader);
            apiTask.Api = api;
            api.ApiTask = apiTask;
            api.ApiTaskKey = apiTask.Id;

            return apiTask;
        }

        private static ApiTaskDto ReadApiTaskFields(SqliteDataReader reader) =>
            new ApiTaskDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Description = (string)reader["description"],
                Api = null,
                Subscriber = null,
                CronTimeExpression = (string)reader["cron_time_expression"],
            };

        private async Task<AggregatorApiDto> ReadAggregationApi(SqliteDataReader reader)
        {
            var id = (int)reader["id"];
            var apiType = (string)reader["api_type"];

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = this.sqlConnection,
            };

            if (apiType == "CoinRanking")
            {
                sqlCommand.CommandText = "SELECT * FROM coin_ranking_apis WHERE id = @apiAggreagatorId";

                const string apiAggregatorIdParameter = "@apiAggreagatorId";
                sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                reader = await sqlCommand.ExecuteReaderAsync();

                return new CoinRankingApiDto
                {
                    Id = id,
                    ApiTaskKey = 0,
                    ApiTask = null,
                    ReferenceCurrency = (string)reader["reference_currency"],
                    SparklineTime = (string)reader["sparkline_time"]
                };
            }
            else if (apiType == "WeatherTracker")
            {
                sqlCommand.CommandText = "SELECT * FROM weather_apis WHERE id = @apiAggreagatorId";

                const string apiAggregatorIdParameter = "@apiAggreagatorId";
                sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                reader = await sqlCommand.ExecuteReaderAsync();

                return new WeatherApiDto
                {
                    Id = id,
                    ApiTaskKey = 0,
                    ApiTask = null,
                    Region = (string)reader["region"],
                };
            }
            else
            {
                sqlCommand.CommandText = "SELECT * FROM covid_aggregator_apis WHERE id = @apiAggreagatorId";

                const string apiAggregatorIdParameter = "@apiAggreagatorId";
                sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                reader = await sqlCommand.ExecuteReaderAsync();

                return new CovidAggregatorApiDto
                {
                    Id = id,
                    ApiTaskKey = 0,
                    ApiTask = null,
                    Country = (string)reader["country"],
                };
            }
        }

        private async Task<int> AddApiTaskAsync(ApiTaskDto apiTask)
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO api_tasks (name, description, subscriber_id, cron_time_expression) " +
                              "VALUES(@name, @description, @subscriber, @cronTime); ",
                Connection = this.sqlConnection,
            };

            AddSqlParametersForApiTask(apiTask, sqlCommand);

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows;
        }

        private async Task<int> AddApiTaskAggregatorAsync(AggregatorApiDto aggregator)
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO apis_aggregators (api_task_key, api_type) " +
                              "VALUES(@apiTask, @apiType); ",
                Connection = this.sqlConnection,
            };

            const string apiTaskIdParameter = "@apiTask";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = aggregator.ApiTaskKey;

            const string descriptionParameter = "@apiType";
            sqlCommand.Parameters.Add(descriptionParameter, SqliteType.Text);
            sqlCommand.Parameters[descriptionParameter].Value = aggregator.ApiType.ToString();

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows;
        }

        private async Task<int> AddConcreteApiTaskAggregatorAsync(AggregatorApiDto aggregator)
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO weather_apis (id, region) " +
                              "VALUES(@id, @region); ",
                Connection = this.sqlConnection,
            };

            const string idParameter = "@id";
            sqlCommand.Parameters.Add(idParameter, SqliteType.Integer);
            sqlCommand.Parameters[idParameter].Value = aggregator.Id;

            if (aggregator.ApiType == ApiTypeDto.WeatherTracker)
            {
                var weatherApi = (WeatherApiDto)aggregator;
                sqlCommand.CommandText = "INSERT INTO weather_apis (id, region) " +
                                         "VALUES(@id, @region); ";

                const string regionParameter = "@region";
                sqlCommand.Parameters.Add(regionParameter, SqliteType.Text);
                sqlCommand.Parameters[regionParameter].Value = weatherApi.Region;
            }
            else if (aggregator.ApiType == ApiTypeDto.WeatherTracker)
            {
                var covidApi = (CovidAggregatorApiDto)aggregator;
                sqlCommand.CommandText = "INSERT INTO covid_aggregator_apis (id, country) " +
                                         "VALUES(@id, @country); ";

                const string countryParameter = "@country";
                sqlCommand.Parameters.Add(countryParameter, SqliteType.Text);
                sqlCommand.Parameters[countryParameter].Value = covidApi.Country;
            }
            else
            {
                var covidApi = (CoinRankingApiDto)aggregator;
                sqlCommand.CommandText = "INSERT INTO coin_ranking_apis (id, sparkline_time, reference_currency) " +
                                         "VALUES(@id, @sparklineTime, @referenceCurrency); ";

                const string sparklineTimeParameter = "@sparklineTime";
                sqlCommand.Parameters.Add(sparklineTimeParameter, SqliteType.Text);
                sqlCommand.Parameters[sparklineTimeParameter].Value = covidApi.SparklineTime;

                const string currencyParameter = "@referenceCurrency";
                sqlCommand.Parameters.Add(currencyParameter, SqliteType.Text);
                sqlCommand.Parameters[currencyParameter].Value = covidApi.ReferenceCurrency;

            }

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows;
        }

        private async Task<int> UpdateApiTaskAsync(ApiTaskDto apiTask)
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "UPDATE api_tasks " +
                              "SET name = @name," +
                              "description = @description," +
                              "cron_time_expression = @cronTime,",
                Connection = this.sqlConnection,
            };

            AddSqlParametersForApiTask(apiTask, sqlCommand);

            await this.sqlConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            await this.sqlConnection.CloseAsync();

            return affectedRows;
        }

        private static void AddSqlParametersForApiTask(ApiTaskDto apiTask, SqliteCommand command)
        {
            const string nameParameter = "@name";
            command.Parameters.Add(nameParameter, SqliteType.Text);
            command.Parameters[nameParameter].Value = apiTask.Name;

            const string descriptionParameter = "@description";
            command.Parameters.Add(descriptionParameter, SqliteType.Text);
            command.Parameters[descriptionParameter].Value = apiTask.Description;

            const string userIdParameter = "@subscriber";
            command.Parameters.Add(userIdParameter, SqliteType.Integer);
            command.Parameters[userIdParameter].Value = apiTask.Subscriber.Id;

            const string cronTimeParameter = "@cronTime";
            command.Parameters.Add(cronTimeParameter, SqliteType.Text);
            command.Parameters[cronTimeParameter].Value = apiTask.CronTimeExpression;
        }
    }
}
