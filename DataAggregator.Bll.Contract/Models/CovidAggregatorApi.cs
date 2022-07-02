namespace DataAggregator.Bll.Contract.Models;

public class CovidAggregatorApi : IAggregatorApi
{
    public int Id { get; set; }

    public string Description { get; set; }

    public string Country { get; set; }
}