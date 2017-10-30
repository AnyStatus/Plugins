using AnyStatus.API;

namespace AnyStatus
{
    public class OpenAppVeyorBuildInBrowser : IOpenInBrowser<AppVeyorBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenAppVeyorBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(AppVeyorBuild item)
        {
            if (item == null || item.IsValid() == false)
                return;

            var url = $"https://ci.appveyor.com/project/{item.AccountName}/{item.ProjectSlug}";

            _processStarter.Start(url);
        }
    }
}
