using AnyStatus.API;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayName("Current Weather (Experimental)")]
    [Description("Experimental. Access current weather data for any location including over 200,000 cities.")]
    public class CurrentWeatherWidgetV1 : Metric, ISchedulable
    {
        private double _value;

        [DisplayName("API Key")]
        [Description("The OpenWeatherMap API key.")]
        public string ApiKey { get; set; }

        [DisplayName("City Id")]
        [Description("The OpenWeatherMap city id.")]
        public string CityId { get; set; }

        [DisplayName("Temperature Scale")]
        public TemperatureScale Scale { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            switch (Scale)
            {
                case TemperatureScale.Celsius:
                    return $"{_value - 273.15:0}°C";

                case TemperatureScale.Fahrenheit:
                    return $"{(_value - 273.15) * 1.8 + 32:0}°F";

                case TemperatureScale.Kelvin:
                    return $"{_value}K";

                default:
                    return _value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
