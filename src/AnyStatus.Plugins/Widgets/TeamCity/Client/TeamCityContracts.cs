using AnyStatus.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AnyStatus
{
    public class TeamCityContracts
    {
        public class BuildDetailsResponse
        {
            public List<BuildDetails> Build { get; set; }
        }

        public class BuildDetails
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

        public class Build
        {
            public BuildType BuildType { get; set; }
        }

        public class BuildType
        {
            [XmlAttribute]
            public string Id { get; set; }
        }
    }
}