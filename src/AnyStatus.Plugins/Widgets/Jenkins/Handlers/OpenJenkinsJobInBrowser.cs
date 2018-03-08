using AnyStatus.API;

namespace AnyStatus
{
#warning Duplicate

    public class OpenJenkinsJobInBrowser : IOpenInBrowser<JenkinsJob_v1>
    {
        private readonly IProcessStarter _processStarter;

        public OpenJenkinsJobInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsJob_v1 item)
        {
            _processStarter.Start(item.URL);
        }
    }
}