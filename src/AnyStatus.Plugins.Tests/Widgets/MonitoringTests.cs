using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class MonitoringTests
    {
        [TestMethod]
        public async Task HttpMonitor()
        {
            var http = new HttpStatus { Url = "http://www.microsoft.com" };
            var request = HealthCheckRequest.Create(http);
            var handler = new HTTPHealthCheck();

            await handler.Handle(request, CancellationToken.None);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }

        [TestMethod]
        public async Task PingHandler()
        {
            var ping = new Ping { Host = "localhost" };
            var request = HealthCheckRequest.Create(ping);
            var handler = new PingHealthCheck();

            await handler.Handle(request, CancellationToken.None);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }

        [TestMethod]
        public void TCPHealthCheck()
        {
            var port = new Port
            {
                Protocol = NetworkProtocol.TCP,
                Host = "www.microsoft.com",
                PortNumber = 80
            };

            var request = HealthCheckRequest.Create(port);
            var handler = new SocketConnectionHealthCheck();

            handler.Handle(request, CancellationToken.None);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }

        [Ignore]
        [TestMethod]
        public void WindowsServiceHandler()
        {
            var request = new WindowsService
            {
                ServiceName = "Dhcp"
            };

            //new WindowsServiceMonitor().Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [Ignore]
        [TestMethod]
        public async Task GitHubIssueHandler()
        {
            var issue = new GitHubIssueV1
            {
                IssueNumber = 1,
                Repository = "AnyStatus",
                Owner = "AnyStatus"
            };

            var request = HealthCheckRequest.Create(issue);

            var handler = new GitHubIssueStateCheck();

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(State.None, request.DataContext.State);
        }

        [TestMethod]
        public async Task CoverallsHandler()
        {
            var widget = new CoverallsCoveredPercent
            {
                Url = "https://coveralls.io/github/xing/hardcover?branch=demo"
            };

            var request = MetricQueryRequest.Create(widget);

            var handler = new CoverallsCoveredPercentQuery();

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(State.None, request.DataContext.State);
            Assert.AreNotSame(State.Error, request.DataContext.State);
        }

        [TestMethod]
        public async Task UptimeRobotOverallStatus()
        {
            var logger = Substitute.For<ILogger>();

            var uptime = new UptimeRobot
            {
                ApiKey = "u131608-259ebe191e11db9a9e47aa51"
            };

            var request = HealthCheckRequest.Create(uptime);
            var handler = new UptimeRobotCheck(logger);

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(State.None, request.DataContext.State);
        }

        [TestMethod]
        public async Task UptimeRobotMonitorStatus()
        {
            var logger = Substitute.For<ILogger>();

            var uptime = new UptimeRobot
            {
                ApiKey = "u131608-259ebe191e11db9a9e47aa51",
                MonitorName = "Blog"
            };

            var request = HealthCheckRequest.Create(uptime);
            var handler = new UptimeRobotCheck(logger);

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(State.None, request.DataContext.State);
        }
    }
}