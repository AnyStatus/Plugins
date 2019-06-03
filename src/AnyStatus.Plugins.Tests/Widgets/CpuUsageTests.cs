using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.Widgets
{
    [TestClass]
    public class CpuUsageTests
    {
        [TestMethod]
        public async Task CpuUsageTest()
        {
            var widget = new CpuUsage();

            var request = MetricQueryRequest.Create(widget);

            var handler = new CpuUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(widget.Value);
        }
    }
}
