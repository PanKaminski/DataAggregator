using Newtonsoft.Json;

namespace DataAggregator.Bll.Infrastructure
{
    internal class WeatherInfo
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public WeatherDetails WeatherDetails { get; set; }

        public override string ToString()
        {
            var location = $"{this.Location.Name},{this.Location.Region}," +
                           $"{this.Location.Country},{this.Location.Latitude},{this.Location.Longitude}" +
                           $"{this.Location.TimeZoneId},{this.Location.LocalTimeEpoch},{this.Location.LocalTime},";
            var updateTime = $"{this.WeatherDetails.LastUpdatedEpoch},{this.WeatherDetails.LastUpdatedTime},";
            var temperature =
                $"{this.WeatherDetails.TemperatureInCelsius},{this.WeatherDetails.TemperatureInFahrenheits}," +
                $"{this.WeatherDetails.IsDay}";
            var description = $"{this.WeatherDetails.Description.Text},{this.WeatherDetails.Description.IconLink}," +
                              $"{this.WeatherDetails.Description.Code},";
            var details = $"{this.WeatherDetails.WindSpeedInMeters},{this.WeatherDetails.WindSpeedInKilometers}," +
                          $"{this.WeatherDetails.WindDegree}, {this.WeatherDetails.WindDirection}" +
                          $"{this.WeatherDetails.Pressure},{this.WeatherDetails.PrecipInInches},{this.WeatherDetails.PrecipInMillimeters}" +
                          $"{this.WeatherDetails.PrecipInInches}, {this.WeatherDetails.Humidity},{this.WeatherDetails.Cloud}," +
                          $"{this.WeatherDetails.FeelsLikeInCelsius}, {this.WeatherDetails.FeelsLikeInFahrenheits}," +
                          $"{this.WeatherDetails.VisInKilometers},{this.WeatherDetails.VisInMiles},{this.WeatherDetails.UV}," +
                          $"{this.WeatherDetails.GustInMph},{this.WeatherDetails.GustInKph}";

            return location + description + updateTime + temperature + details;
        }
    }
}
