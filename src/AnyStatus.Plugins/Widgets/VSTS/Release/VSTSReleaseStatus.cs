using AnyStatus.API;
using AnyStatus.API.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

//todo: unautherized state when 401
//https://www.visualstudio.com/en-us/docs/integrate/api/rm/contracts#ReleaseStatus

namespace AnyStatus
{
    public class VSTSReleaseStatus : ICheckHealth<VSTSRelease_v1>
    {
        public async Task Handle(HealthCheckRequest<VSTSRelease_v1> request, CancellationToken cancellationToken)
        {
            var client = new VstsClient(new VstsConnection());

            request.DataContext.MapTo(client.Connection);

            if (request.DataContext.DefinitionId == null)
            {
                var definition = await client.GetReleaseDefinitionAsync(request.DataContext.ReleaseDefinitionName)
                    .ConfigureAwait(false);

                request.DataContext.DefinitionId = definition.Id;
            }

            var latestRelease = await client.GetLatestReleaseAsync(request.DataContext.DefinitionId.Value)
                .ConfigureAwait(false);

            var releaseDetails = await client.GetReleaseDetailsAsync(latestRelease.Id)
                .ConfigureAwait(false);

            RemoveEnvironments(request.DataContext, releaseDetails);

            AddEnvironments(request.DataContext, releaseDetails);
        }

        private static void RemoveEnvironments(VSTSRelease_v1 node, VSTSReleaseDetails releaseDetails)
        {
            if (node == null || node.Items == null)
                throw new InvalidOperationException();

            var environments = node.Items
                .Where(k => !releaseDetails.Environments.Any(e => e.Name == k.Name))
                .ToList();

            foreach (var environment in environments)
            {
                Application.Current.Dispatcher.Invoke(() => node.Remove(environment));
            }
        }

        private static void AddEnvironments(VSTSRelease_v1 vstsRelease, VSTSReleaseDetails releaseDetails)
        {
            if (vstsRelease == null || vstsRelease.Items == null)
                throw new InvalidOperationException();

            foreach (var environment in releaseDetails.Environments)
            {
                var node = vstsRelease.Items.FirstOrDefault(i => i.Name == environment.Name);

                if (node == null)
                {
                    node = new VSTSReleaseEnvironment
                    {
                        Name = environment.Name,
                        EnvironmentId = environment.Id
                    };

                    Application.Current.Dispatcher.Invoke(() => vstsRelease.Add(node));
                }

                node.State = environment.State;
            }
        }
    }
}