namespace DataAggregator.Bll.Contract.Models;

public class WeatherApi : IAggregatorApi
{
    public int Id { get; set; }

    public ApiType Name => ApiType.SportOdds;

    public string Description { get; set; }

    public string ApiUrl { get; set; }

    public string Region { get; set; }
}