using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts
{

    public class AzureDevOpsBuild
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
}