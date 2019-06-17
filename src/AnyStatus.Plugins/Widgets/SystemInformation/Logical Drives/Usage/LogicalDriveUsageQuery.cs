using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class LogicalDriveUsageQuery : IMetricQuery<LogicalDriveUsage>
    {
        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<LogicalDriveUsage> request, CancellationToken cancellationToken)
        {
            request.DataContext.Progress = GetDriveUsageAsync(request.DataContext.Drive, request.DataContext.PercentageType);

            request.DataContext.State = State.Ok;

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private int GetDriveUsageAsync(string drive, PercentageType percentageType)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return DriveInformation.GetDriveUsedPercentage(drive);
                case PercentageType.PercentageRemaining:
                    return DriveInformation.GetDriveAvailablePercentage(drive);
                default:
                    throw new NotImplementedException($"Not implemented the percentage type of [{percentageType}]");
            }
        }
    }
}