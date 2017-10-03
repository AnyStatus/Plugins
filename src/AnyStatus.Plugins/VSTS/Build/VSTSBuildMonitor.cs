using AnyStatus.API;
using System.Diagnostics;

namespace AnyStatus
{
    public class VSTSBuildMonitor : IMonitor<VSTSBuild_v1>
    {
        [DebuggerStepThrough]
        public void Handle(VSTSBuild_v1 vstsBuild)
        {
            var client = new VSTSClient
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
                var definition = client.GetBuildDefinitionAsync(vstsBuild.DefinitionName).Result;

                vstsBuild.DefinitionId = definition.Id;
            }

            var latestBuild = client.GetLatestBuildAsync(vstsBuild.DefinitionId.Value).Result;

            vstsBuild.State = latestBuild.State;
        }
    }
}
