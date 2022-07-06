namespace DataAggregator.Bll.Contract.Models
{
    public class CoinRankingApi : AggregatorApi
    {
        private string referenceCurrency;

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
