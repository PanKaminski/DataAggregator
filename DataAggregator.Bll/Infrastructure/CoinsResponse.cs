using Newtonsoft.Json;

namespace DataAggregator.Bll.Infrastructure
{
    internal class CoinsResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public CoinsData CoinsData { get; set; }
    }

    internal class CoinsData
    {
        [JsonProperty("stats")]
        public CoinsRankingStatistics Statistics { get; set; }

        [JsonProperty("coins")]
        public IEnumerable<CoinInfo> Coins { get; set; }
    }

    internal class CoinsRankingStatistics
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("totalMarkets")]
        public int TotalMarkets { get; set; }

        [JsonProperty("totalExchanges")]
        public int TotalExchanges { get; set; }

        [JsonProperty("totalCoins")]
        public string TotalMarketCap { get; set; }

        [JsonProperty("total24hVolume")]
        public string TotalPeriodicalVolume { get; set; }
    }
}
