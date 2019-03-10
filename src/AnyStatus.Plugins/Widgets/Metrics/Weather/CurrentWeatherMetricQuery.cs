using AnyStatus.API;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class CurrentWeatherMetricQuery : IMetricQuery<CurrentWeatherWidgetV1>
    {
        public async Task Handle(MetricQueryRequest<CurrentWeatherWidgetV1> request, CancellationToken cancellationToken)
        {
            var restClient = new RestClient("https://api.openweathermap.org/data/2.5/weather");

            var restRequest = new RestRequest()
                .AddParameter("id", request.DataContext.CityId)
                .AddParameter("APPID", request.DataContext.ApiKey);

            var response = await restClient.ExecuteTaskAsync<CurrentWeather>(restRequest, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessful && double.TryParse(response.Data.Main["temp"], out var temp))
            {
                char symbol;

                switch (request.DataContext.Scale)
                {
                    case TemperatureScale.Celsius:
                        temp = temp - 273.15;
                        symbol = 'C';
                        break;
                    case TemperatureScale.Fahrenheit:
                        temp = (temp - 273.15) * 1.8 + 32;
                        symbol = 'F';
                        break;
                    default:
                        symbol = 'K';
                        break;
                }

                request.DataContext.Value = $"{Math.Round(temp)}°{symbol}";
                request.DataContext.State = State.Ok;
            }
            else
            {
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                request.DataContext.State = State.Error;
            }
        }
    }
}
