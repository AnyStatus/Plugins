using AnyStatus.API;
using System.Diagnostics;

namespace AnyStatus
{
    public class JenkinsJobMonitor : IMonitor<JenkinsJob_v1>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsJob_v1 item)
        {
            var jenkinsClient = new JenkinsClient();

            var build = jenkinsClient.GetJobAsync(item).Result;

            if (build.IsRunning)
            {
                OnBuildRunning(item, build.Executor.Progress);
                return;
            }

            if (item.ShowProgress) item.ResetProgress();

            item.State = ConvertBuildResultToState(build.Result);
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