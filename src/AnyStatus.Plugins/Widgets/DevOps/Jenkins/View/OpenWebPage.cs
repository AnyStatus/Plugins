using AnyStatus.API;

namespace AnyStatus
{
    public class OpenJenkinsViewWebPage : OpenWebPage<JenkinsView_v1>
    {
        public OpenJenkinsViewWebPage(IProcessStarter ps) : base(ps) { }
    }
}