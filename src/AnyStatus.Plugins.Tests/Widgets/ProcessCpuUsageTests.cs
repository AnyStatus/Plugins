using System;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Plugins.Tests.Widgets
{
    [TestClass]
    public class ProcessCpuUsageTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var widget = new ProcessCpuUsage()
            {
                 ProcessName = "System",
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCpuUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsNotNull(widget.Value);
        }
    }
}
