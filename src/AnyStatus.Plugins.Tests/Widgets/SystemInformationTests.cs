using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.Widgets
{
    [TestClass]
    public class SystemInformationTests
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

        [TestMethod]
        public async Task ProcessCpuUsageTest()
        {
            var widget = new ProcessCpuUsage()
            {
                ProcessName = "System",
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCpuUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ProcessCpuUsage_ProcessNotFound_Test()
        {
            var widget = new ProcessCpuUsage()
            {
                ProcessName = "DoesNotExist",
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCpuUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task RamUsageTest()
        {
            var widget = new RamUsage();

            var request = MetricQueryRequest.Create(widget);

            var handler = new RamUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task ProcessCountTest()
        {
            var widget = new ProcessCount();

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCountQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task ThreadCountTest()
        {
            var widget = new ThreadCount();

            var request = MetricQueryRequest.Create(widget);

            var handler = new ThreadCountQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.IsTrue(widget.Value > 0);
        }
    }
}
