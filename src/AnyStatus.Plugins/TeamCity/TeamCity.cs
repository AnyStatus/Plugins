using AnyStatus.API;
using System.Collections.Generic;

namespace AnyStatus
{
    public class TeamCityBuildDetailsResponse
    {
        public List<TeamCityBuildDetails> Build { get; set; }
    }

    public class TeamCityBuildDetails
    {
        public bool Running { get; set; }

        public string Status { get; set; }

        public string StatusText { get; set; }

        public State State
        {
            get
            {
                if (Running) return State.Running;

                switch (Status)
                {
                    case "SUCCESS":
                        return State.Ok;
                    case "FAILURE":
                    case "ERROR":
                        return State.Failed;
                    case "UNKNOWN":
                    default:
                        return State.Unknown;
                }
            }
        }
    }
}
