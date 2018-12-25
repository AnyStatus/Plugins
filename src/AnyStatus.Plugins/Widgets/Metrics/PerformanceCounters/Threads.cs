using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName(nameof(Threads))]
    public class Threads : PerformanceCounter
    {
        public Threads()
        {
            Name = nameof(Threads);
        }

        [XmlIgnore]
        [ReadOnly(true)]
        public new string CounterName => "Threads";

        [XmlIgnore]
        [ReadOnly(true)]
        public new string CategoryName => "System";

        [XmlIgnore]
        [Browsable(false)]
        public new string InstanceName { get; }
    }

    public class ThreadsQuery : BasePerformanceCounterQuery, IMetricQuery<Threads>
    {
        public Task Handle(MetricQueryRequest<Threads> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext);
        }
    }
}
