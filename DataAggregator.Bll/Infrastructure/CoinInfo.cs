using Newtonsoft.Json;

namespace DataAggregator.Bll.Infrastructure
{
    internal class CoinInfo
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("24hVolume")]
        public string Volume { get; set; }

        [JsonProperty("marketCap")]
        public string MarketCap { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("btcPrice")]
        public string BitCoinPrice { get; set; }

        [JsonProperty("change")]
        public string Change { get; set; }

        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("sparkline")]
        public IEnumerable<string> Sparkline { get; set; }

        [JsonProperty("coinrankingUrl")]
        public string CoinRankingUrl { get; set; }

        [JsonProperty("tier")]
        public int Tier { get; set; }

        [JsonProperty("lowVolume")]
        public bool IsLowVolume { get; set; }

        [JsonProperty("listedAt")]
        public int ListedAt { get; set; }

        public override string ToString() => this.Uuid + "," + this.Symbol + "," + this.Name + ","
                                             + this.Color + "," + this.IconUrl + "," + this.Volume + ","
                                             + this.MarketCap + "," + this.Price + "," + BitCoinPrice + ","
                                             + this.Change + "," + this.Rank + "," + this.CoinRankingUrl + ","
                                             + this.Tier + "," + this.IsLowVolume + "," + this.ListedAt + ","
                                             + string.Join(",", this.Sparkline);
    }
}