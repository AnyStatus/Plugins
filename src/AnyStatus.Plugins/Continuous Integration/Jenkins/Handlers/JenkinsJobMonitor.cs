using AnyStatus.API;
using System;
using System.Diagnostics;

//todo: change handler to async

namespace AnyStatus
{
    public class JenkinsJobMonitor : IMonitor<JenkinsJob_v1>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsJob_v1 jenkinsJobPlugin)
        {
            var jenkinsClient = new JenkinsClient();

            var jenkinsJob = jenkinsClient.GetJobAsync(jenkinsJobPlugin).Result;

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
#warning verify status string
                case "QUEUED":
                    return State.Queued;
                default:
                    return State.Unknown;
            }
        }

        private static void OnBuildRunning(JenkinsJob_v1 item, int progress)
        {
            item.Progress = progress;

            item.State = State.Running;

            if (item.ShowProgress == false) item.ShowProgress = true;
        }
    }
}