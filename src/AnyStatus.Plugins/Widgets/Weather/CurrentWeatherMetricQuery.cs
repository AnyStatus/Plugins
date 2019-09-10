using AnyStatus.API;
using RestSharp;
using System;
using System.Globalization;
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

            if (response.IsSuccessful && double.TryParse(response.Data.Main["temp"], NumberStyles.Number, new CultureInfo("en-US"), out var temperature))
            {
                request.DataContext.Value = temperature;
                request.DataContext.State = State.Ok;
            }
            else
            {
                throw new Exception("An error occurred while getting temperature.", response.ErrorException);
            }
        }
    }
}
