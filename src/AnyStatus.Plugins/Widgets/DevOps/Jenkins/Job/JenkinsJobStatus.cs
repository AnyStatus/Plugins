using AnyStatus.API;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class JenkinsJobMonitor : ICheckHealth<JenkinsJob_v1>
    {
        private readonly ILogger _logger;

        public JenkinsJobMonitor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException();
        }

        public async Task Handle(HealthCheckRequest<JenkinsJob_v1> request, CancellationToken cancellationToken)
        {
            var jenkinsClient = new JenkinsClient(_logger);

            var jenkinsJob = await jenkinsClient.GetJobAsync(request.DataContext).ConfigureAwait(false);

            if (jenkinsJob.IsRunning)
            {
                OnBuildRunning(request.DataContext, jenkinsJob.Executor.Progress);
                return;
            }

            if (request.DataContext.ShowProgress)
            {
                request.DataContext.ShowProgress = false;
                request.DataContext.Progress = 0;
            }

            if (jenkinsJob.Result == null)
                throw new Exception("Jenkins job result is null.");

            request.DataContext.State = ConvertBuildResultToState(jenkinsJob.Result);
        }

        private static State ConvertBuildResultToState(string result)
        {
            switch (result)
            {
                case "SUCCESS":
                    return State.Ok;
                case "ABORTED":
                    return State.Canceled;
                case "FAILURE":
                    return State.Failed;
                case "UNSTABLE":
                    return State.PartiallySucceeded;
                case "QUEUED": //todo: verify status string or replace with a different response property.
                    return State.Queued;
                default:
                    return State.Unknown;
            }
        }

        private static void OnBuildRunning(JenkinsJob_v1 plugin, int progress)
        {
            plugin.Progress = progress;

            plugin.State = State.Running;

            if (!plugin.ShowProgress)
            {
                plugin.ShowProgress = true;
            }
        }
    }
}