using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.VSTS.Build
{
    public class VstsBuildTrigger : ITriggerBuild<VSTSBuild_v1>
    {
        public async Task HandleAsync(VSTSBuild_v1 build)
        {
            var client = new VstsClient(new VstsConnection());

            build.MapTo(client.Connection);

            if (build.DefinitionId == null)
            {
                var definition = await client.GetBuildDefinitionAsync(build.DefinitionName).ConfigureAwait(false);

                build.DefinitionId = definition.Id;
            }

            await client.QueueNewBuild(build.DefinitionId.Value).ConfigureAwait(false);
        }
    }
}
