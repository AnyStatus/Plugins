using AnyStatus.API;

namespace AnyStatus
{
    public class OpenJenkinsJobInBrowser : OpenWebPage<JenkinsJob_v1>
    {
        public OpenJenkinsJobInBrowser(IProcessStarter ps) : base(ps)
        {
        }
    }
}