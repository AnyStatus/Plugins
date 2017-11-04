using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Diagnostics;

namespace AnyStatus
{
    public class TeamCityBuildMonitor : IMonitor<TeamCityBuild>
    {
        [DebuggerStepThrough]
        public void Handle(TeamCityBuild teamCityBuid)
        {
            var teamCityClient = new TeamCityClient(new TeamCityConnection());

            teamCityBuid.MapTo(teamCityClient.Connection);

            var buildDetails = teamCityClient.GetBuildDetailsAsync(teamCityBuid).Result;

            teamCityBuid.State = buildDetails.State;
            teamCityBuid.StateText = buildDetails.StatusText;
        }
    }
}
