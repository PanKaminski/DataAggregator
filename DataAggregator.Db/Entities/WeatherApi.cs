namespace DataAggregator.Db.Entities;

public class WeatherApi : IAggregatorApi
{
    public int Id { get; set; }

    public string Description { get; set; }

    public string Region { get; set; }
}