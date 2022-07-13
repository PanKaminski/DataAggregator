using System.Data;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Dal.Contract.Repositories;
using Microsoft.Data.Sqlite;

namespace DataAggregator.Dal.Repositories
{
    public class ApiTasksRepository : IApiTasksRepository
    {
        private readonly string connectionString;

        public ApiTasksRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddAsync(ApiTaskDto apiTask)
        {
            if (apiTask is null)
            {
                throw new ArgumentNullException(nameof(apiTask));
            }

            var result = await AddApiTaskAsync(apiTask);
            apiTask.Id = await this.GetLastInsertedRowIdAsync();
            apiTask.Api.ApiTaskKey = apiTask.Id;
            apiTask.Api.Id = apiTask.Id;
            await AddApiTaskAggregatorAsync(apiTask.Api);
            await AddConcreteApiTaskAggregatorAsync(apiTask.Api);

            return result > 0 ? apiTask.Id : -1;
        }

        public async IAsyncEnumerable<ApiTaskDto> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User id must be greater than zero.", nameof(userId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
                CommandText = "SELECT * FROM api_tasks WHERE subscriber_id = @userId"
            };

            const string userIdParameter = "@userId";
            sqlCommand.Parameters.Add(userIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[userIdParameter].Value = userId;

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return await CreateApiTaskAsync(reader, dbConnection);
            }
        }

        public async IAsyncEnumerable<ApiTaskDto> GetAsync()
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
                CommandText = "SELECT * FROM api_tasks"
            };

            await dbConnection.OpenAsync();

            await using var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return await CreateApiTaskAsync(reader, dbConnection);
            }
        }

        public async Task<ApiTaskDto> GetAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Api task id must be greater than zero.", nameof(apiTaskId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand()
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT * FROM api_tasks WHERE id = @id;",
                Connection = dbConnection,
            };

            const string apiTaskIdParameter = "@id";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTaskId;

            await dbConnection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new KeyNotFoundException("Api task with such id wasn't found.");
            }

            await reader.ReadAsync();

            return await CreateApiTaskAsync(reader, dbConnection);
        }

        public async Task<bool> DeleteAsync(int apiTaskId)
        {
            if (apiTaskId <= 0)
            {
                throw new ArgumentException("Api task id must be greater than zero.", nameof(apiTaskId));
            }

            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
                CommandText = "DELETE FROM api_tasks WHERE id=@apiTaskId"
            };

            const string apiTaskIdParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTaskId;

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync() > 0;
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

            apiTask.Id = apiTaskId;

            return await UpdateApiTaskAsync(apiTask) >= 0;
        }

        private async Task<ApiTaskDto> CreateApiTaskAsync(SqliteDataReader reader, SqliteConnection dbConnection)
        {
            var apiTask = ReadApiTaskFields(reader);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
                CommandText = "SELECT * FROM apis_aggregators WHERE api_task_key = @apiTaskId;"
            };

            const string apiTaskIdParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = apiTask.Id;

            await using var aggregationApiReader = await sqlCommand.ExecuteReaderAsync();

            await aggregationApiReader.ReadAsync();

            var api = await ReadAggregationApi(aggregationApiReader, dbConnection);

            apiTask.Api = api;
            api.ApiTask = apiTask;
            api.ApiTaskKey = apiTask.Id;

            return apiTask;
        }

        private static ApiTaskDto ReadApiTaskFields(SqliteDataReader reader) =>
            new ApiTaskDto
            {
                Id = (int)(long)reader["id"],
                Name = (string)reader["name"],
                Description = (string)reader["description"],
                Api = null,
                Subscriber = null,
                CronTimeExpression = (string)reader["cron_time_expression"],
            };

        private async Task<AggregatorApiDto> ReadAggregationApi(SqliteDataReader reader, SqliteConnection dbConnection)
        {
            var id = (int)(long)reader["id"];
            var apiType = (string)reader["api_type"];

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
            };

            switch (apiType)
            {
                case "CoinRanking":
                {
                    sqlCommand.CommandText = "SELECT * FROM coin_ranking_apis WHERE id = @apiAggreagatorId";

                    const string apiAggregatorIdParameter = "@apiAggreagatorId";
                    sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                    sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                    reader = await sqlCommand.ExecuteReaderAsync();
                    await reader.ReadAsync();

                    return new CoinRankingApiDto
                    {
                        Id = id,
                        ApiTaskKey = 0,
                        ApiTask = null,
                        ApiType = ApiTypeDto.CoinRanking,
                        ReferenceCurrency = (string)reader["reference_currency"],
                        SparklineTime = (string)reader["sparkline_time"]
                    };
                }
                case "WeatherTracker":
                {
                    sqlCommand.CommandText = "SELECT * FROM weather_apis WHERE id = @apiAggreagatorId";

                    const string apiAggregatorIdParameter = "@apiAggreagatorId";
                    sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                    sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                    reader = await sqlCommand.ExecuteReaderAsync();
                    await reader.ReadAsync();

                    return new WeatherApiDto
                    {
                        Id = id,
                        ApiTaskKey = 0,
                        ApiTask = null,
                        ApiType = ApiTypeDto.WeatherTracker,
                        Region = (string)reader["region"],
                    };
                }
                default:
                {
                    sqlCommand.CommandText = "SELECT * FROM covid_aggregator_apis WHERE id = @apiAggreagatorId";

                    const string apiAggregatorIdParameter = "@apiAggreagatorId";
                    sqlCommand.Parameters.Add(apiAggregatorIdParameter, SqliteType.Integer);
                    sqlCommand.Parameters[apiAggregatorIdParameter].Value = id;

                    reader = await sqlCommand.ExecuteReaderAsync();
                    await reader.ReadAsync();

                    return new CovidAggregatorApiDto
                    {
                        Id = id,
                        ApiTaskKey = 0,
                        ApiType = ApiTypeDto.CovidTracker,
                        ApiTask = null,
                        Country = (string)reader["country"],
                    };
                }
            }
        }

        private async Task<int> AddApiTaskAsync(ApiTaskDto apiTask)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO api_tasks (name, description, subscriber_id, cron_time_expression) " +
                              "VALUES(@name, @description, @subscriber, @cronTime); ",
                Connection = dbConnection,
            };

            AddSqlParametersForApiTask(apiTask, sqlCommand);

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task<int> GetLastInsertedRowIdAsync()
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "SELECT last_insert_rowid();",
                Connection = dbConnection,
            };

            await dbConnection.OpenAsync();
            return (int)(long)(await sqlCommand.ExecuteScalarAsync());
        }

        private async Task<int> AddApiTaskAggregatorAsync(AggregatorApiDto aggregator)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO apis_aggregators (id, api_task_key, api_type) " +
                              "VALUES(@id, @apiTask, @apiType);",
                Connection = dbConnection,
            };

            const string idParameter = "@id";
            sqlCommand.Parameters.Add(idParameter, SqliteType.Integer);
            sqlCommand.Parameters[idParameter].Value = aggregator.Id;

            const string apiTaskIdParameter = "@apiTask";
            sqlCommand.Parameters.Add(apiTaskIdParameter, SqliteType.Integer);
            sqlCommand.Parameters[apiTaskIdParameter].Value = aggregator.ApiTaskKey;

            const string descriptionParameter = "@apiType";
            sqlCommand.Parameters.Add(descriptionParameter, SqliteType.Text);
            sqlCommand.Parameters[descriptionParameter].Value = aggregator.ApiType.ToString();

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task<int> AddConcreteApiTaskAggregatorAsync(AggregatorApiDto aggregator)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
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
            else if (aggregator.ApiType == ApiTypeDto.CovidTracker)
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

            await dbConnection.OpenAsync();

            return await sqlCommand.ExecuteNonQueryAsync();
        }

        private async Task<int> UpdateApiTaskAsync(ApiTaskDto apiTask)
        {
            await using var dbConnection = new SqliteConnection(this.connectionString);

            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                CommandText = "UPDATE api_tasks " +
                              "SET name = @name," +
                              "description = @description," +
                              "cron_time_expression = @cronTime " +
                              "WHERE id=@apiTaskId;",
                Connection = dbConnection,
            };

            const string idParameter = "@apiTaskId";
            sqlCommand.Parameters.Add(idParameter, SqliteType.Integer);
            sqlCommand.Parameters[idParameter].Value = apiTask.Id;

            AddSqlParametersForApiTask(apiTask, sqlCommand);

            await dbConnection.OpenAsync();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();

            if (affectedRows < 1)
            {
                return affectedRows;
            }

            return await UpdateTaskAggregatorAsync(apiTask.Api, dbConnection);
        }

        private async Task<int> UpdateTaskAggregatorAsync(AggregatorApiDto aggregator, SqliteConnection dbConnection)
        {
            await using var sqlCommand = new SqliteCommand
            {
                CommandType = CommandType.Text,
                Connection = dbConnection,
            };

            const string idParameter = "@id";
            sqlCommand.Parameters.Add(idParameter, SqliteType.Integer);
            sqlCommand.Parameters[idParameter].Value = aggregator.Id;

            if (aggregator.ApiType == ApiTypeDto.WeatherTracker)
            {
                var weatherApi = (WeatherApiDto)aggregator;
                sqlCommand.CommandText = "UPDATE weather_apis " +
                                         "SET region = @region " +
                                         "WHERE id = @id;";

                const string regionParameter = "@region";
                sqlCommand.Parameters.Add(regionParameter, SqliteType.Text);
                sqlCommand.Parameters[regionParameter].Value = weatherApi.Region;
            }
            else if (aggregator.ApiType == ApiTypeDto.CovidTracker)
            {
                var covidApi = (CovidAggregatorApiDto)aggregator;
                sqlCommand.CommandText = "UPDATE covid_aggregator_apis" +
                                         "SET country = @country " +
                                         "WHERE id = @id;";

                const string countryParameter = "@country";
                sqlCommand.Parameters.Add(countryParameter, SqliteType.Text);
                sqlCommand.Parameters[countryParameter].Value = covidApi.Country;
            }
            else
            {
                var covidApi = (CoinRankingApiDto)aggregator;
                sqlCommand.CommandText = "UPDATE coin_ranking_apis SET " +
                                         "sparkline_time = @sparklineTime, " +
                                         "reference_currency = @referenceCurrency " +
                                         "WHERE id = @id; ";

                const string sparklineTimeParameter = "@sparklineTime";
                sqlCommand.Parameters.Add(sparklineTimeParameter, SqliteType.Text);
                sqlCommand.Parameters[sparklineTimeParameter].Value = covidApi.SparklineTime;

                const string currencyParameter = "@referenceCurrency";
                sqlCommand.Parameters.Add(currencyParameter, SqliteType.Text);
                sqlCommand.Parameters[currencyParameter].Value = covidApi.ReferenceCurrency;
            }

            return await sqlCommand.ExecuteNonQueryAsync();
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
