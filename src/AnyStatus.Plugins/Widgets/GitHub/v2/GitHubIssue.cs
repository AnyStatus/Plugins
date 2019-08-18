using AnyStatus.API;
using RestSharp.Deserializers;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    public class GitHubIssue
    {
        public string Title { get; set; }

        public string HtmlUrl { get; internal set; }

        public int Number { get; internal set; }

        [DeserializeAs(Name = "state")]
        public string StateText { get; set; }

        public State State
        {
            get
            {
                switch (StateText)
                {
                    case "open": return State.Open;
                    case "closed": return State.Closed;
                    default: return State.Unknown;
                }
            }
        }
    }
}