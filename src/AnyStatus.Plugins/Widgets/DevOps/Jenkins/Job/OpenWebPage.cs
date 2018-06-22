using AnyStatus.API;

namespace AnyStatus
{
    public class OpenJenkinsJobWebPage : OpenWebPage<JenkinsJob_v1>
    {
        public OpenJenkinsJobWebPage(IProcessStarter ps) : base(ps) { }
    }
}