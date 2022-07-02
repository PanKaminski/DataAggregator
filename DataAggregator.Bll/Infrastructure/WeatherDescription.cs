using Newtonsoft.Json;

namespace DataAggregator.Bll.Infrastructure;

internal class WeatherDescription
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("icon")]
    public string IconLink { get; set; }

    [JsonProperty("int")]
    public int Code { get; set; }
}