using AnyStatus.API;
using System;

namespace AnyStatus
{
    public class LogicalDriveUsageQuery : RequestHandler<MetricQueryRequest<LogicalDriveUsage>>
    {
        protected override void HandleCore(MetricQueryRequest<LogicalDriveUsage> request)
        {
            var driveInformation = DriveInformation.ReadDrive(request.DataContext.Drive);

            var percent = GetDrivePercentage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.Value = percent;
            request.DataContext.Progress = percent;
            request.DataContext.Message = GetDriveMessage(driveInformation, request.DataContext.PercentageType);
            request.DataContext.State = GetDriveState(driveInformation, request.DataContext.PercentageType, request.DataContext.ErrorPercentage);
        }

        private static int GetDrivePercentage(DriveInformation driveInformation, PercentageType percentageType)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return driveInformation.UsedPercentage;

                case PercentageType.PercentageRemaining:
                    return driveInformation.AvailablePercentage;

                default:
                    throw new NotImplementedException($"Percentage type \"{percentageType}\" is not supported.");
            }
        }

        private static string GetDriveMessage(DriveInformation driveInformation, PercentageType percentageType)
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

        private static State GetDriveState(DriveInformation driveInformation, PercentageType percentageType, int errorPercentage)
        {
            switch (percentageType)
            {
                case PercentageType.PercentageUsed:
                    return driveInformation.UsedPercentage >= errorPercentage ? State.Failed : State.Ok;

                case PercentageType.PercentageRemaining:
                    return driveInformation.AvailablePercentage <= errorPercentage ? State.Failed : State.Ok;

                default:
                    throw new NotImplementedException($"Percentage type \"{percentageType}\" is not supported.");
            }
        }
    }
}