using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    [DisplayName("Uptime Robot")]
    [DisplayColumn("Monitoring")]
    public class UptimeRobot : Item, IScheduledItem
    {
        private const string Category = "Uptime Robot";

        [Required]
        [DisplayName("API Key")]
        [Category(Category)]
        [Description("Required. Uptime Robot API key. You can get the key from \"My Settings\" page.")]
        public string ApiKey { get; set; }

        [DisplayName("Monitor Name")]
        [Category(Category)]
        [Description("Optional. Leave empty for the overall status of all monitors.")]
        public string MonitorName { get; set; }
    }

    [Obsolete("API has changed to v2")]
    public class UptimeRobotMonitor : IMonitor<UptimeRobot>
    {
        private const string UptimeRobotApi = "https://api.uptimerobot.com";
        private const string ConstApiParams = "format=json&noJsonCallback=1";

        private readonly ILogger _logger;

        public UptimeRobotMonitor(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        [DebuggerStepThrough]
        public void Handle(UptimeRobot uptimeRobot)
        {
            UptimeRobotMonitorStatus status;

            if (string.IsNullOrEmpty(uptimeRobot.MonitorName))
            {
                var monitors = GetMonitors(uptimeRobot);

                status = monitors.Max(k => k.status);
            }
            else
            {
                var monitor = GetMonitor(uptimeRobot);

                status = monitor.status;
            }

            uptimeRobot.State = GetState(status);
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

        private static Monitor GetMonitor(UptimeRobot uptimeRobot)
        {
            var monitorsResponse = SendMonitorsRequest(uptimeRobot.ApiKey, uptimeRobot.MonitorName);

            if (monitorsResponse.stat == "fail")
            {
                throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");
            }

            return monitorsResponse.monitors.monitor.First();
        }

        [DebuggerStepThrough]
        private static List<Monitor> GetMonitors(UptimeRobot uptimeRobot)
        {
            var total = 1;
            var monitors = new List<Monitor>();

            while (monitors.Count < total)
            {
                var monitorsResponse = SendMonitorsRequest(uptimeRobot.ApiKey, monitors.Count);

                if (monitorsResponse.stat == "fail")
                    throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");

                monitors.AddRange(monitorsResponse.monitors.monitor);

                total = monitorsResponse.total;
            }

            return monitors;
        }

        private static MonitorsResponse SendMonitorsRequest(string apiKey, string monitorName)
        {
            var uri = new Uri($"{UptimeRobotApi}/getMonitors?apiKey={apiKey}&{ConstApiParams}&limit=1&search={monitorName}");

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(uri).Result;

                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;

                return new JavaScriptSerializer().Deserialize<MonitorsResponse>(content);
            }
        }

        [DebuggerStepThrough]
        private static MonitorsResponse SendMonitorsRequest(string apiKey, int offset)
        {
            var uri = new Uri($"{UptimeRobotApi}/getMonitors?apiKey={apiKey}&{ConstApiParams}&offset={offset}");

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(uri).Result;

                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;

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

        #endregion
    }
}
