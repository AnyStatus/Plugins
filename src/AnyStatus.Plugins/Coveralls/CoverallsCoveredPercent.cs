using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Coveralls")]
    [DisplayColumn("Continuous Integration")]
    [Description("Coveralls covered code percentage")]
    public class CoverallsCoveredPercent : Metric, IMonitored, ICanOpenInBrowser
    {
        private const string Category = "Coveralls";

        public CoverallsCoveredPercent()
        {
            Threshold = 80;
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [Description("Coveralls repository URL address. For example: https://coveralls.io/github/AlonAm/AnyStatus")]
        public string Url { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [Description("Failed threshold.")]
        public int Threshold { get; set; }
    }

    public class CoverallsCoveredPercentMonitor : IMonitor<CoverallsCoveredPercent>
    {
        [DebuggerStepThrough]
        public void Handle(CoverallsCoveredPercent item)
        {
            string endpoint = GetEndpoint(item);

            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.GetAsync(endpoint).Result;

                httpResponse.EnsureSuccessStatusCode();

                var content = httpResponse.Content.ReadAsStringAsync().Result;

                var response = new JavaScriptSerializer()
                        .Deserialize<CoveredPercentResponse>(content);

                item.Value = response.CoveredPercent + "%";

                item.State = response.CoveredPercent < item.Threshold ? State.Failed : State.Ok;
            }
        }

        private static string GetEndpoint(CoverallsCoveredPercent item)
        {
            const string json = ".json";

            var uri = new Uri(item.Url);

            return uri.GetLeftPart(UriPartial.Path) + json + uri.Query;
        }

        #region Contracts

        class CoveredPercentResponse
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

        #endregion
    }

    public class OpenCoverallsInBrowser : IOpenInBrowser<CoverallsCoveredPercent>
    {
        private readonly IProcessStarter _processStarter;

        public OpenCoverallsInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(CoverallsCoveredPercent item)
        {
            _processStarter.Start(item.Url);
        }
    }
}
