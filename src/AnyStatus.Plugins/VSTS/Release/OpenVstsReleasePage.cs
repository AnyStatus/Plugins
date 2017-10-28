using AnyStatus.API;
using System;

namespace AnyStatus
{
    public class OpenVstsReleasePage : IOpenInBrowser<VSTSRelease_v1>
    {
        private readonly IProcessStarter _processStarter;

        public OpenVstsReleasePage(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(VSTSRelease_v1 release)
        {
            if (release.DefinitionId == null)
                throw new InvalidOperationException("Cannot open web page. Unknown definition id.");

            var uri = $"https://{release.Account}.visualstudio.com/{release.Project}/_release/index?definitionId={release.DefinitionId}&_a=releases";

            _processStarter.Start(uri);
        }
    }
}
