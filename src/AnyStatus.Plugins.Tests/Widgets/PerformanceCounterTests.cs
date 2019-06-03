using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class PerformanceCounterTests
    {
        [TestMethod]
        public async Task PerformanceCounter()
        {
            var http = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes",
            };

            var request = MetricQueryRequest.Create(http);

            var handler = new PerformanceCounterQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreNotEqual(0, request.DataContext.Value);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }
    }
}
