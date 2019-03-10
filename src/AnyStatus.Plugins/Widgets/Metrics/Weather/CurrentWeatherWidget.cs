using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("Current Weather (Experimental)")]
    [Description("Experimental. Access current weather data for any location including over 200,000 cities.")]
    public class CurrentWeatherWidgetV1 : Metric, ISchedulable
    {
        [DisplayName("API Key")]
        [Description("The OpenWeatherMap API key.")]
        public string ApiKey { get; set; }

        [DisplayName("City Id")]
        [Description("The OpenWeatherMap city id.")]
        public string CityId { get; set; }

        [DisplayName("Temperature Scale")]
        public TemperatureScale Scale { get; set; }
    }
}
