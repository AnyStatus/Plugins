﻿using AnyStatus.API;

namespace AnyStatus
{
    public class ProcessCountQuery : RequestHandler<MetricQueryRequest<ProcessCount>>
    {
        private const string CategoryName = "System";
        private const string CounterName = "Processes";
        private const string InstanceName = "";

        protected override void HandleCore(MetricQueryRequest<ProcessCount> request)
        {
            using (var counter = string.IsNullOrWhiteSpace(request.DataContext.MachineName)
                ? new System.Diagnostics.PerformanceCounter(CategoryName, CounterName)
                : new System.Diagnostics.PerformanceCounter(CategoryName, CounterName, InstanceName, request.DataContext.MachineName))
            {
                request.DataContext.Value = (int)counter.NextValue();
                request.DataContext.State = State.Ok;
            }
        }
    }
}