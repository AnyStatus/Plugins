using AnyStatus.API;
using System;

namespace AnyStatus
{
    public class OpenVstsBuildPage : IOpenInBrowser<VSTSBuild_v1>
    {
        private readonly IProcessStarter _processStarter;

        public OpenVstsBuildPage(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(VSTSBuild_v1 build)
        {
            if (build.DefinitionId == null)
                throw new InvalidOperationException("Cannot open web page. Unknown build definition id.");

            var uri = $"https://{build.Account}.visualstudio.com/{build.Project}/_build/index?definitionId={build.DefinitionId}&_a=completed";

            _processStarter.Start(uri);
        }
    }
}
