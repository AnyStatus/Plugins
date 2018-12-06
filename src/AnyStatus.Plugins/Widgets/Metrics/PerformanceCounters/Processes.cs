using AnyStatus.API;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    public class Processes : PerformanceCounter
    {
        [XmlIgnore] private new string Name => nameof(Processes);
        [XmlIgnore] private new string CounterName => nameof(Processes);
        [XmlIgnore] private new string CategoryName => "System";
        [XmlIgnore] private new string InstanceName { get; }
        [XmlIgnore] private new string MachineName { get; }
    }

    public class ProcessesQuery : PerformanceCounterQuery, IMetricQuery<Processes>
    {
        public Task Handle(MetricQueryRequest<Processes> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }
    }
}
