using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class PingdomHealthCheck : ICheckHealth<Pingdom>
    {
        private const string ServerAddress = "https://api.pingdom.com/api/2.0/checks/";

        [DebuggerStepThrough]
        public async Task Handle(HealthCheckRequest<Pingdom> request, CancellationToken cancellationToken)
        {
            PingdomCheckStatus status;

            if (string.IsNullOrWhiteSpace(request.DataContext.CheckId))
            {
                //todo: support paging. request.DataContext api limits the number of results.

                var checkList = await GetChecks<CheckList>(ServerAddress, request.DataContext.UserName, request.DataContext.Password, request.DataContext.ApiKey).ConfigureAwait(false);

                status = checkList.Checks.Max(k => k.Status);
            }
            else
            {
                var check = await GetChecks<Check>(ServerAddress + request.DataContext.CheckId, request.DataContext.UserName, request.DataContext.Password, request.DataContext.ApiKey).ConfigureAwait(false);

                status = check.Status;
            }

            request.DataContext.State = GetState(status);
        }

        private State GetState(PingdomCheckStatus status)
        {
            switch (status)
            {
                case PingdomCheckStatus.Up:
                    return State.Ok;

                case PingdomCheckStatus.Paused:
                    //todo: add Paused state
                    return State.Canceled;

                case PingdomCheckStatus.Unknown:
                    return State.Unknown;

                case PingdomCheckStatus.Unconfirmed_down:
                case PingdomCheckStatus.Down:
                    return State.Failed;

                default:
                    return State.Unknown;
            }
        }

        private async Task<T> GetChecks<T>(string endpoint, string username, string password, string apiKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                client.DefaultRequestHeaders.Add("App-Key", apiKey);

                var httpResponse = await client.GetAsync(ServerAddress).ConfigureAwait(false);

                httpResponse.EnsureSuccessStatusCode();

                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        #region Contracts

        public enum PingdomCheckStatus
        {
            Up,
            Paused,
            Unknown,
            Unconfirmed_down,
            Down,
        }

        public class CheckList
        {
            public IEnumerable<Check> Checks { get; set; }
        }

        public class Check
        {
            public PingdomCheckStatus Status { get; set; }
        }

        #endregion Contracts
    }
}