using AnyStatus.API;
using AnyStatus.API.Utils;

namespace AnyStatus
{
    //todo: convert to async handler

    public class VstsBuildMonitor : IMonitor<VSTSBuild_v1>
    {
        public void Handle(VSTSBuild_v1 vstsBuild)
        {
            var vstsClient = new VstsClient(new VstsConnection());

            vstsBuild.MapTo(vstsClient.Connection);

            if (vstsBuild.DefinitionId == null)
            {
                var definition = vstsClient.GetBuildDefinitionAsync(vstsBuild.DefinitionName).Result;

                vstsBuild.DefinitionId = definition.Id;
            }

            var latestBuild = vstsClient.GetLatestBuildAsync(vstsBuild.DefinitionId.Value).Result;

            vstsBuild.State = latestBuild != null ? latestBuild.State : State.Unknown;
        }
    }
}