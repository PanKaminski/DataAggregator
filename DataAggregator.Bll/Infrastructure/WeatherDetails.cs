using Newtonsoft.Json;

namespace DataAggregator.Bll.Infrastructure;

internal class WeatherDetails
{
    [JsonProperty("last_updated_epoch")]
    public long LastUpdatedEpoch { get; set; }

    [JsonProperty("last_updated")]
    public string LastUpdatedTime { get; set; }

    [JsonProperty("temp_c")]
    public double TemperatureInCelsius { get; set; }

    [JsonProperty("temp_f")]
    public double TemperatureInFahrenheits { get; set; }

    [JsonProperty("is_day")]
    public int IsDay { get; set; }

    [JsonProperty("condition")]
    public WeatherDescription Description { get; set; }

    [JsonProperty("wind_mph")]
    public double WindSpeedInMeters { get; set; }

    [JsonProperty("wind_kph")]
    public double WindSpeedInKilometers { get; set; }

    [JsonProperty("wind_degree")]
    public int WindDegree { get; set; }

    [JsonProperty("wind_dir")]
    public string WindDirection { get; set; }

    [JsonProperty("pressure_mb")]
    public double Pressure { get; set; }

    [JsonProperty("pressure_in")]
    public double PressureInInches { get; set; }

    [JsonProperty("precip_mm")]
    public double PrecipInMillimeters { get; set; }

    [JsonProperty("precip_in")]
    public double PrecipInInches { get; set; }

    [JsonProperty("humidity")]
    public int Humidity { get; set; }

    [JsonProperty("cloud")]
    public int Cloud { get; set; }

    [JsonProperty("feelslike_c")]
    public double FeelsLikeInCelsius { get; set; }

    [JsonProperty("feelslike_f")]
    public double FeelsLikeInFahrenheits { get; set; }

    [JsonProperty("vis_km")]
    public double VisInKilometers { get; set; }

    [JsonProperty("vis_miles")]
    public double VisInMiles { get; set; }

    [JsonProperty("uv")]
    public double UV { get; set; }

    [JsonProperty("gust_mph")]
    public double GustInMph { get; set; }

    [JsonProperty("gust_kph")]
    public double GustInKph { get; set; }
}