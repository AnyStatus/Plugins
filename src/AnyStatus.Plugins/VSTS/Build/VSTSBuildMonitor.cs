using AnyStatus.API;
using System.Diagnostics;

namespace AnyStatus
{
    public class VSTSBuildMonitor : IMonitor<VSTSBuild_v1>
    {
        public void Handle(VSTSBuild_v1 vstsBuild)
        {
            var vstsClient = new VSTSClient
            {
                Connection = new VSTSConnection
                {
                    Account = vstsBuild.Account,
                    Project = vstsBuild.Project,
                    UserName = vstsBuild.UserName,
                    Password = vstsBuild.Password,
                }
            };

            if (vstsBuild.DefinitionId == null)
            {
                var definition = vstsClient.GetBuildDefinitionAsync(vstsBuild.DefinitionName).Result;

                vstsBuild.DefinitionId = definition.Id;
            }

            var latestBuild = vstsClient.GetLatestBuildAsync(vstsBuild.DefinitionId.Value).Result;

            vstsBuild.State = latestBuild.State;
        }
    }
}
