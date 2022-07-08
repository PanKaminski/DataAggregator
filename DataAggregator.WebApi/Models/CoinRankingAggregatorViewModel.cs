namespace DataAggregator.WebApi.Models
{
    public class CoinRankingAggregatorViewModel : ApiAggregatorViewModel
    {
        private string referenceCurrency;

        public string SparklineTime { get; set; }

        public string ReferenceCurrency { get; set; }
    }
}
