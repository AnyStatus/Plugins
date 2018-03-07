using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public class CoverallsCoveredPercentQuery : IMetricQuery<CoverallsCoveredPercent>
    {
        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<CoverallsCoveredPercent> request, CancellationToken cancellationToken)
        {
            string endpoint = GetEndpoint(request.DataContext);

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(endpoint).ConfigureAwait(false);

                httpResponse.EnsureSuccessStatusCode();

                var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                var response = new JavaScriptSerializer()
                        .Deserialize<CoveredPercentResponse>(content);

                request.DataContext.Value = response.CoveredPercent + "%";

                request.DataContext.State = response.CoveredPercent < request.DataContext.Threshold ? State.Failed : State.Ok;
            }
        }

        private static string GetEndpoint(CoverallsCoveredPercent item)
        {
            const string json = ".json";

            var uri = new Uri(item.URL);

            return uri.GetLeftPart(UriPartial.Path) + json + uri.Query;
        }

        

        #region Contracts

        private class CoveredPercentResponse
        {
            public float covered_percent { private get; set; }

            public int CoveredPercent
            {
                get
                {
                    return (int)covered_percent;
                }
            }
        }

        #endregion Contracts
    }
}