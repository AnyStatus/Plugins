using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnyStatus
{
    public class BatteryStatusQuery : IMetricQuery<Battery>
    {
        public Task Handle(MetricQueryRequest<Battery> request, CancellationToken cancellationToken)
        {
            var power = SystemInformation.PowerStatus;

            request.DataContext.Value = power.BatteryLifePercent.ToString("P0");
            request.DataContext.Progress = (int)(power.BatteryLifePercent * 100);
            request.DataContext.Message = $"{power.BatteryLifeRemaining / 3600} hr {power.BatteryLifeRemaining % 3600 / 60} min remaining";
            request.DataContext.State = power.BatteryLifePercent * 100 >= request.DataContext.BatteryLifePercentThreshold ? State.Ok : State.Failed;

            return Task.CompletedTask;
        }
    }
}