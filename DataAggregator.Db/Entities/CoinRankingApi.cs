﻿namespace DataAggregator.Db.Entities
{
    public class CoinRankingApi : IAggregatorApi
    {
        private string referenceCurrency;

        public int Id { get; set; }

        public string Description { get; set; }

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
