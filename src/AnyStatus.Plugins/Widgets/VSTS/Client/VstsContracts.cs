using AnyStatus.API;
using System.Collections.Generic;
using System.Linq;

namespace AnyStatus
{
    public class VSTSBuildDefinition
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class VSTSBuild
    {
        public string Result { get; set; }

        public string Status { get; set; }

        public State State
        {
            get
            {
                switch (Status)
                {
                    case "notStarted":
                        return State.Queued;

                    case "inProgress":
                        return State.Running;
                }

                switch (Result)
                {
                    case "notStarted":
                        return State.Running;

                    case "succeeded":
                        return State.Ok;

                    case "failed":
                        return State.Failed;

                    case "partiallySucceeded":
                        return State.PartiallySucceeded;

                    case "canceled":
                        return State.Canceled;

                    default:
                        return State.Unknown;
                }
            }
        }
    }

    public class Collection<T>
    {
        public int Count { get; set; }

        public List<T> Value { get; set; }
    }

    public class VSTSRelease
    {
        public long Id { get; set; }
    }

    public class VSTSReleaseDetails : VSTSRelease
    {
        public ReleaseEnvironment[] Environments { get; set; }
    }

    public class ReleaseEnvironment
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
                        return PreDeployApprovals.Any(k => k.Status != "approved") ? State.Unknown : State.Running;

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
                        return State.Unknown;
                }
            }
        }

        public ReleasePreDeployApproval[] PreDeployApprovals { get; set; }
    }

    public class VSTSReleaseDefinition
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class ReleasePreDeployApproval
    {
        public string Status { get; set; }
    }
}