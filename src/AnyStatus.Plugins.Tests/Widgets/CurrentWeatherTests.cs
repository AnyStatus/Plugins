using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class CurrentWeatherTests
    {
        private const string ApiKey = "56d878861dd420d0ba408011d6c8514d";
        private const string CityId = "293397";

        [TestMethod]
        public async Task CurrentWeatherCelsiusTest()
        {
            var widget = new CurrentWeatherWidgetV1
            {
                ApiKey = ApiKey,
                CityId = CityId,
                Scale = TemperatureScale.Celsius
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new CurrentWeatherMetricQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsTrue(widget.ToString().Contains("°C"));
        }

        [TestMethod]
        public async Task CurrentWeatherFahrenheitTest()
        {
            var widget = new CurrentWeatherWidgetV1
            {
                ApiKey = ApiKey,
                CityId = CityId,
                Scale = TemperatureScale.Fahrenheit
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new CurrentWeatherMetricQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsTrue(widget.ToString().Contains("°F"));
        }
    }
}
