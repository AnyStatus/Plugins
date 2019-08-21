using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

            Assert.AreEqual(State.Ok, widget.State);
        }

        [TestMethod]
        public async Task ProcessCpuUsageTest()
        {
            var widget = new ProcessCpuUsage
            {
                ProcessName = "System",
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCpuUsageQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ProcessCpuUsage_ProcessNotFound_Test()
        {
            var widget = new ProcessCpuUsage
            {
                ProcessName = "DoesNotExist"
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

            Assert.AreEqual(State.Ok, widget.State);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task ProcessCountTest()
        {
            var widget = new ProcessCount();

            var request = MetricQueryRequest.Create(widget);

            var handler = new ProcessCountQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task ThreadCountTest()
        {
            var widget = new ThreadCount();

            var request = MetricQueryRequest.Create(widget);

            var handler = new ThreadCountQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task PerformanceCounterTest()
        {
            var widget = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes",
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new PerformanceCounterQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);

            Assert.IsTrue(widget.Value > 0);
        }

        [TestMethod]
        public async Task BatteryTest()
        {
            var widget = new Battery();

            var request = MetricQueryRequest.Create(widget);

            var handler = new BatteryStatusQuery();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);

            Assert.IsNotNull(widget.Value);

            Assert.IsNotNull(widget.Message);

            Assert.IsTrue(widget.Progress > 0);
        }

        [TestMethod]
        public async Task FileExistsTest()
        {
            var widget = new FileExists
            {
                Path = Assembly.GetExecutingAssembly().Location,
            };

            var request = HealthCheckRequest.Create(widget);

            await new FileExistsCheck().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);
        }

        [TestMethod]
        public async Task FileNotExistsTest()
        {
            var widget = new FileExists
            {
                Path = "",
            };

            var request = HealthCheckRequest.Create(widget);

            await new FileExistsCheck().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Failed, widget.State);
        }

        [TestMethod]
        public async Task LogicalDriveUsage_PercentageUsed_Test()
        {
            var drive = DriveInfo.GetDrives().First();

            var widget = new LogicalDriveUsage
            {
                Drive = drive.Name,
                ErrorPercentage = 90,
                PercentageType = PercentageType.PercentageUsed,
            };

            var request = MetricQueryRequest.Create(widget);

            await new LogicalDriveUsageQuery().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);
        }

        [TestMethod]
        public async Task LogicalDriveUsage_PercentageUsed_Failed_Test()
        {
            var drive = DriveInfo.GetDrives().First();

            var widget = new LogicalDriveUsage
            {
                Drive = drive.Name,
                ErrorPercentage = 0,
                PercentageType = PercentageType.PercentageUsed,
            };

            var request = MetricQueryRequest.Create(widget);

            await new LogicalDriveUsageQuery().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Failed, widget.State);
        }

        [TestMethod]
        public async Task LogicalDriveUsage_PercentageRemaining_Test()
        {
            var drive = DriveInfo.GetDrives().First();

            var widget = new LogicalDriveUsage
            {
                Drive = drive.Name,
                ErrorPercentage = 0,
                PercentageType = PercentageType.PercentageRemaining,
            };

            var request = MetricQueryRequest.Create(widget);

            await new LogicalDriveUsageQuery().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Ok, widget.State);
        }

        [TestMethod]
        public async Task LogicalDriveUsage_PercentageRemaining_Failed_Test()
        {
            var drive = DriveInfo.GetDrives().First();

            var widget = new LogicalDriveUsage
            {
                Drive = drive.Name,
                ErrorPercentage = 100,
                PercentageType = PercentageType.PercentageRemaining,
            };

            var request = MetricQueryRequest.Create(widget);

            await new LogicalDriveUsageQuery().Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Failed, widget.State);
        }


    }
}
