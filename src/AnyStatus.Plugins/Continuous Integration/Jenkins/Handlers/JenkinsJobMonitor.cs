using AnyStatus.API;
using System;

//todo: change handler to async

namespace AnyStatus
{
    public class JenkinsJobMonitor : IMonitor<JenkinsJob_v1>
    {
        private readonly IJenkinsClient _jenkinsClient;

        public JenkinsJobMonitor(IJenkinsClient jenkinsClient)
        {
            _jenkinsClient = Preconditions.CheckNotNull(jenkinsClient, nameof(jenkinsClient));
        }

        public void Handle(JenkinsJob_v1 jenkinsJobPlugin)
        {
            var jenkinsJob = _jenkinsClient.GetJobAsync(jenkinsJobPlugin).Result;

            if (jenkinsJob.IsRunning)
            {
                OnBuildRunning(jenkinsJobPlugin, jenkinsJob.Executor.Progress); return;
            }

            if (jenkinsJobPlugin.ShowProgress) jenkinsJobPlugin.ResetProgress();

            if (jenkinsJob.Result == null) throw new Exception("Jenkins job result is null.");

            jenkinsJobPlugin.State = ConvertBuildResultToState(jenkinsJob.Result);
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
#warning verify status string. Note, there is a dedicate property for that.
                case "QUEUED":
                    return State.Queued;
                default:
                    return State.Unknown;
            }
        }

        private static void OnBuildRunning(JenkinsJob_v1 plugin, int progress)
        {
            plugin.Progress = progress;

            plugin.State = State.Running;

            if (plugin.ShowProgress == false) plugin.ShowProgress = true;
        }
    }
}