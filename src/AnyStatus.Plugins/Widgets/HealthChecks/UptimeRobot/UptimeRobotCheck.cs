using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class UptimeRobotCheck : ICheckHealth<UptimeRobot>
    {
        private const string UptimeRobotApi = "https://api.uptimerobot.com";
        private const string ConstApiParams = "format=json&noJsonCallback=1";

        private readonly ILogger _logger;

        public UptimeRobotCheck(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<UptimeRobot> request, CancellationToken cancellationToken)
        {
            UptimeRobotMonitorStatus status;

            if (string.IsNullOrEmpty(request.DataContext.MonitorName))
            {
                var monitors = await GetMonitors(request.DataContext).ConfigureAwait(false);

                status = monitors.Max(k => k.status);
            }
            else
            {
                var monitor = await GetMonitor(request.DataContext).ConfigureAwait(false);

                status = monitor.status;
            }

            request.DataContext.State = GetState(status);
        }

        private static State GetState(UptimeRobotMonitorStatus status)
        {
            switch (status)
            {
                case UptimeRobotMonitorStatus.Pause:
                    return State.Disabled;

                case UptimeRobotMonitorStatus.NotChecked:
                    return State.None;

                case UptimeRobotMonitorStatus.Up:
                    return State.Ok;

                case UptimeRobotMonitorStatus.SeemsDown:
                case UptimeRobotMonitorStatus.Down:
                    return State.Failed;

                default:
                    return State.Unknown;
            }
        }

        private async Task<Monitor> GetMonitor(UptimeRobot uptimeRobot)
        {
            var monitorsResponse = await SendMonitorsRequest(uptimeRobot.ApiKey, uptimeRobot.MonitorName).ConfigureAwait(false);

            if (monitorsResponse.stat == "fail")
            {
                throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");
            }

            return monitorsResponse.monitors.monitor.First();
        }

        private async Task<IEnumerable<Monitor>> GetMonitors(UptimeRobot uptimeRobot)
        {
            var total = 1;
            var monitors = new List<Monitor>();

            while (monitors.Count < total)
            {
                var monitorsResponse = await SendMonitorsRequest(uptimeRobot.ApiKey, monitors.Count).ConfigureAwait(false);

                if (monitorsResponse.stat == "fail")
                    throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");

                monitors.AddRange(monitorsResponse.monitors.monitor);

                total = monitorsResponse.total;
            }

            return monitors;
        }

        private async Task<MonitorsResponse> SendMonitorsRequest(string apiKey, string monitorName)
        {
            var uri = new Uri($"{UptimeRobotApi}/getMonitors?apiKey={apiKey}&{ConstApiParams}&limit=1&search={monitorName}");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<MonitorsResponse>(content);
            }
        }

        private async Task<MonitorsResponse> SendMonitorsRequest(string apiKey, int offset)
        {
            var uri = new Uri($"{UptimeRobotApi}/getMonitors?apiKey={apiKey}&{ConstApiParams}&offset={offset}");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<MonitorsResponse>(content);
            }
        }

        #region Contracts

        public class Monitor
        {
            public UptimeRobotMonitorStatus status { get; set; }
        }

        public class Monitors
        {
            public List<Monitor> monitor { get; set; }
        }

        public class MonitorsResponse
        {
            public string stat { get; set; }
            public int total { get; set; }
            public string message { get; set; }
            public Monitors monitors { get; set; }
        }

        public enum UptimeRobotMonitorStatus
        {
            Pause = 0,
            NotChecked = 1,
            Up = 2,
            SeemsDown = 8,
            Down = 9
        }

        #endregion Contracts
    }
}