using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class LogicalDriveUsageQuery : IMetricQuery<LogicalDriveUsage>
    {
        private static readonly string NL = Environment.NewLine;

        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<LogicalDriveUsage> request, CancellationToken cancellationToken)
        {
            DriveInformation driveInformation = DriveInformation.ReadDrive(request.DataContext.Drive);

            request.DataContext.Progress = GetDrivePercentage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.Message = GetDriveMessage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.State = State.Ok;

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private int GetDrivePercentage(DriveInformation driveInformation, PercentageType percentageType)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return driveInformation.UsedPercentage;
                case PercentageType.PercentageRemaining:
                    return driveInformation.AvailablePercentage;
                default:
                    throw new NotImplementedException($"Not implemented the percentage type of [{percentageType}]");
            }
        }

        private string GetDriveMessage(DriveInformation driveInformation, PercentageType percentageType)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return $"{driveInformation.Drive} - {driveInformation.UsedPercentage}%{NL}" +
                           $"{driveInformation.TotalNumberOfUsedGigabytes}GB used out of {driveInformation.TotalNumberOfGigabytes}GB";
                case PercentageType.PercentageRemaining:
                    return $"{driveInformation.Drive} - {driveInformation.AvailablePercentage}%{NL}" +
                           $"{driveInformation.TotalNumberOfFreeGigabytes}GB available out of {driveInformation.TotalNumberOfGigabytes}GB";
                default:
                    throw new NotImplementedException($"Not implemented the percentage type of [{percentageType}]");
            }
        }
    }
}