using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class CurrentWeatherTests
    {
        [TestMethod]
        public async Task CurrentWeatherTest()
        {
            var widget = new CurrentWeatherWidgetV1
            {
                ApiKey = "56d878861dd420d0ba408011d6c8514d",
                CityId = "293397",
                Scale = TemperatureScale.Celsius
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new CurrentWeatherMetricQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(widget.Value);
        }
    }
}
