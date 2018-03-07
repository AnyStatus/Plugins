using AnyStatus.API;

namespace AnyStatus
{
    public class OpenAppVeyorBuildWebPage : OpenWebPage<AppVeyorBuild>
    {
        public OpenAppVeyorBuildWebPage(IProcessStarter ps) : base(ps) { }
    }
}