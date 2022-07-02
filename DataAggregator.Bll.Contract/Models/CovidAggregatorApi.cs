namespace DataAggregator.Bll.Contract.Models;

public class CovidAggregatorApi : IAggregatorApi
{
    public int Id { get; set; }

    public ApiType Name => ApiType.Covid;

    public string Description { get; set; }

    public string ApiUrl { get; set; }

    public IEnumerable<ApiTask> Subscriptions { get; set; }

    public string Country { get; set; }
}