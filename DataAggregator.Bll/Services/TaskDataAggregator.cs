using System.Reflection;
using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.Bll.Infrastructure;
using Newtonsoft.Json;

namespace DataAggregator.Bll.Services
{
    public class TaskDataAggregator : IDataAggregator
    {
        private readonly HttpClient httpClient;

        public TaskDataAggregator(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> ReceiveDataAsync(IAggregatorApi task) => task switch 
        {
            CovidAggregatorApi api => await ReceiveDataAsync(api),
            CoinRankingApi api => await ReceiveDataAsync(api),
            WeatherApi api => await ReceiveDataAsync(api),
            _ => throw new InvalidOperationException("Unknown api.")
        };

        private async Task<string> ReceiveDataAsync(CovidAggregatorApi task)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://covid-19-tracking.p.rapidapi.com/v1/{task.Country.ToLower()}"),
                Headers =
                {
                    { "X-RapidAPI-Key", "2784faac60msh442834adae43258p110a3bjsndb73bec1ddbd" },
                    { "X-RapidAPI-Host", "covid-19-tracking.p.rapidapi.com" },
                },
            };

            using var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var covidData = JsonConvert.DeserializeObject<CovidInfo>(body);
            
            var propertyNames = typeof(CovidInfo).GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jpa => jpa.PropertyName ?? string.Empty)
                .Aggregate((cur, next) => cur + ',' + next);

            await using var csvWriter = new StringWriter();
            await csvWriter.WriteLineAsync(propertyNames);
            await csvWriter.WriteLineAsync(covidData?.ToString() ?? string.Empty);

            var content = csvWriter.ToString();

            return content;
        }

        private async Task<string> ReceiveDataAsync(CoinRankingApi task)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://coinranking1.p.rapidapi.com/coins?referenceCurrencyUuid={task.ReferenceCurrency}&timePeriod={task.SparklineTime}&tiers%5B0%5D=1&orderBy=marketCap&orderDirection=desc&limit=50&offset=0"),
                Headers =
                {
                    { "X-RapidAPI-Key", "2784faac60msh442834adae43258p110a3bjsndb73bec1ddbd" },
                    { "X-RapidAPI-Host", "coinranking1.p.rapidapi.com" },
                },
            };

            using var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var coinsData = JsonConvert.DeserializeObject<CoinsResponse>(body);

            var columnNames = typeof(CoinInfo).GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jp => jp.PropertyName)
                .Where(pn => pn != "sparkline")
                .Aggregate((cur, next) => cur + ',' + next);

            columnNames += coinsData?.CoinsData?.Coins?.FirstOrDefault()?
                .Sparkline.Select((sl, i) => "sparkline" + i).Aggregate((cur, next) => cur + "," + next);

            await using var csvWriter = new StringWriter();
            await csvWriter.WriteLineAsync(columnNames);

            foreach (var coin in coinsData!.CoinsData.Coins)
            {
                await csvWriter.WriteLineAsync(coin.ToString());
            }

            await csvWriter.WriteLineAsync(coinsData.ToString() ?? string.Empty);

            var content = csvWriter.ToString();

            return content;
        }

        private async Task<string> ReceiveDataAsync(WeatherApi task)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://weatherapi-com.p.rapidapi.com/current.json?q={task.Region}"),
                Headers =
                {
                    { "X-RapidAPI-Key", "2784faac60msh442834adae43258p110a3bjsndb73bec1ddbd" },
                    { "X-RapidAPI-Host", "weatherapi-com.p.rapidapi.com" },
                },
            };

            using var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var weatherData = JsonConvert.DeserializeObject<WeatherInfo>(body);

            var locationColumns = typeof(Location).GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jpa => jpa?.PropertyName ?? string.Empty)
                .Aggregate((cur, next) => cur + ',' + next);

            var descriptionColumns = typeof(WeatherDescription).GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jpa => jpa?.PropertyName ?? string.Empty)
                .Aggregate((cur, next) => cur + ',' + next);

            var detailsColumns = typeof(WeatherDetails).GetProperties()
                .Select(p => p.GetCustomAttribute<JsonPropertyAttribute>())
                .Select(jp => jp?.PropertyName ?? string.Empty)
                .Aggregate((cur, next) => next != "condition" ? cur + ',' + next : cur);

            await using var csvWriter = new StringWriter();
            await csvWriter.WriteLineAsync(locationColumns + "," + descriptionColumns + "," + detailsColumns);
            await csvWriter.WriteLineAsync(weatherData?.ToString() ?? string.Empty);

            var content = csvWriter.ToString();

            return content;
        }
    }
}
