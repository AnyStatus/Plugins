using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayColumn("Metrics")]
    [DisplayName(nameof(Processes))]
    public class Processes : PerformanceCounter
    {
        public Processes()
        {
            Name = nameof(Processes);
        }

        [XmlIgnore]
        [ReadOnly(true)]
        public new string CounterName => "Processes";

        [XmlIgnore]
        [ReadOnly(true)]
        public new string CategoryName => "System";

        [XmlIgnore]
        [Browsable(false)]
        public new string InstanceName { get; }
    }

    public class ProcessesQuery : BasePerformanceCounterQuery, IMetricQuery<Processes>
    {
        public Task Handle(MetricQueryRequest<Processes> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }
    }
}
