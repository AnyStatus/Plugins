using AnyStatus.API;

namespace AnyStatus
{
    public class OpenTeamCityBuildInBrowser : IOpenInBrowser<TeamCityBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenTeamCityBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(TeamCityBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Url) || string.IsNullOrEmpty(item.BuildTypeId))
                return;

            var url = $"{item.Url}/viewType.html?buildTypeId={item.BuildTypeId}";

            _processStarter.Start(url);
        }
    }
}