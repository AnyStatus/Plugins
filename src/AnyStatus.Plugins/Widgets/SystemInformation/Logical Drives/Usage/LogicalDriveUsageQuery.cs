using AnyStatus.API;
using System;

namespace AnyStatus
{
    public class LogicalDriveUsageQuery : RequestHandler<MetricQueryRequest<LogicalDriveUsage>>
    {
        protected override void HandleCore(MetricQueryRequest<LogicalDriveUsage> request)
        {
            var driveInformation = DriveInformation.ReadDrive(request.DataContext.Drive);

            request.DataContext.Progress = GetDrivePercentage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.Message = GetDriveMessage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.State = GetDriveState(driveInformation, request.DataContext.PercentageType, request.DataContext.ErrorPercentage);
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
                    throw new NotImplementedException($"Percentage type \"{percentageType}\" is not supported");
            }
        }

        private string GetDriveMessage(DriveInformation driveInformation, PercentageType percentageType)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return $"{driveInformation.Drive} - {driveInformation.UsedPercentage}%{Environment.NewLine}" +
                           $"{driveInformation.TotalNumberOfUsedGigabytes} GB used out of {driveInformation.TotalNumberOfGigabytes} GB";

                case PercentageType.PercentageRemaining:
                    return $"{driveInformation.Drive} - {driveInformation.AvailablePercentage}%{Environment.NewLine}" +
                           $"{driveInformation.TotalNumberOfFreeGigabytes} GB available out of {driveInformation.TotalNumberOfGigabytes} GB";

                default:
                    throw new NotImplementedException($"Percentage type \"{percentageType}\" is not supported.");
            }
        }

        private State GetDriveState(DriveInformation driveInformation, PercentageType percentageType, int errorPercentage)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return driveInformation.UsedPercentage >= errorPercentage ? State.Error : State.Ok;

                case PercentageType.PercentageRemaining:
                    return driveInformation.AvailablePercentage <= errorPercentage ? State.Error : State.Ok;

                default:
                    throw new NotImplementedException($"Percentage type \"{percentageType}\" is not supported.");
            }
        }
    }
}