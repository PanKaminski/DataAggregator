namespace DataAggregator.Bll.Contract.Models
{
    public class CoinRankingApi : AggregatorApi
    {
        private string referenceCurrency;

        public string SparklineTime { get; set; }

        public string ReferenceCurrency
        {
            get => this.referenceCurrency switch
            {
                "5k-_VTxqtCEI" => "Euro",
                "Qwsogvtv82FCd" => "Bitcoin",
                "yhjMzLPhuIDl" => "US Dollar",
                _ => string.Empty
            };
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
