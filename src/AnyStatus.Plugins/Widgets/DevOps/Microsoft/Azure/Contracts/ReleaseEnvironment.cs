using System.Linq;
using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts
{

    public class AzureDevOpsReleaseEnvironment
    {
        public long Id { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public State State
        {
            get
            {
                //https://www.visualstudio.com/en-us/docs/integrate/api/rm/contracts#EnvironmentStatus

                switch (Status)
                {
                    case "notStarted":
                        return State.None;

                    case "inProgress":
                        return PreDeployApprovals.Any(k => k.Status != "approved") ? State.None : State.Running;

                    case "succeeded":
                        return State.Ok;

                    case "canceled":
                        return State.Canceled;

                    case "rejected":
                        return State.Failed;

                    case "queued":
                        return State.Queued;

                    case "scheduled":
                        return State.None;

                    case "partiallySucceeded":
                        return State.PartiallySucceeded;

                    default:
                        return State.None;
                }
            }
        }

        public AzureDevOpsReleasePreDeployApproval[] PreDeployApprovals { get; set; }
    }
}