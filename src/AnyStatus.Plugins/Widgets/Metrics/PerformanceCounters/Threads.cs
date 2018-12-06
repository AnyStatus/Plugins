using AnyStatus.API;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    public class Threads : PerformanceCounter
    {
        [XmlIgnore] private new string Name => nameof(Threads);
        [XmlIgnore] private new string CounterName => nameof(Threads);
        [XmlIgnore] private new string CategoryName => "System";
        [XmlIgnore] private new string InstanceName { get; }
        [XmlIgnore] private new string MachineName { get; }
    }

    public class ThreadsQuery : PerformanceCounterQuery, IMetricQuery<Threads>
    {
        public Task Handle(MetricQueryRequest<Threads> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }
    }
}
