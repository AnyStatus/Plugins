using AnyStatus.API;

namespace AnyStatus
{
    public class OpenTeamCityBuildWebPage : OpenWebPage<TeamCityBuild>
    {
        public OpenTeamCityBuildWebPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}