namespace DataAggregator.WebApi.Models
{
    public class CoinRankingApiTaskRequest : ApiTaskCreateRequest
    {
        public CoinRankingAggregatorViewModel Api { get; set; }
    }
}
