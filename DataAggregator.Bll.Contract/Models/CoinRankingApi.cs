namespace DataAggregator.Bll.Contract.Models
{
    public class CoinRankingApi : IAggregatorApi
    {
        private string referenceCurrency;
        public int Id { get; set; }

        public ApiType Name { get; }

        public string Description { get; set; }

        public string ApiUrl { get; set; }

        public IEnumerable<ApiTask> Subscriptions { get; set; }

        public string SparklineTime { get; set; }

        public string ReferenceCurrency
        {
            get => this.referenceCurrency;
            set
            {
                this.referenceCurrency = value switch
                {
                    "Euro" => "5k-_VTxqtCEI",
                    "Bitcoin" => "Qwsogvtv82FCd",
                    _ => "yhjMzLPhuIDl"
                };
            }
        }
    }
}
