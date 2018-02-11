using AnyStatus.API;

namespace AnyStatus
{
    public class OpenJenkinsViewInBrowser : IOpenInBrowser<JenkinsView_v1>
    {
        private readonly IProcessStarter _processStarter;

        public OpenJenkinsViewInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsView_v1 jenkinsView)
        {
            _processStarter.Start(jenkinsView.URL);
        }
    }
}