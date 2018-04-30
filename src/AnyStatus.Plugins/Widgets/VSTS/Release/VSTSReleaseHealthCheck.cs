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
    public class VSTSReleaseHealthCheck : ICheckHealth<VSTSRelease_v1>
    {
        public async Task Handle(HealthCheckRequest<VSTSRelease_v1> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext;

            var client = new VstsClient(new VstsConnection());

            widget.MapTo(client.Connection);

            if (widget.DefinitionId == null)
            {
                var definition = await client
                    .GetReleaseDefinitionAsync(widget.ReleaseDefinitionName)
                        .ConfigureAwait(false);

                widget.DefinitionId = definition.Id;
            }

            var latestRelease = await client
                .GetLastReleaseAsync(widget.DefinitionId.Value)
                    .ConfigureAwait(false);

            var releaseDetails = await client
                .GetReleaseDetailsAsync(latestRelease.Id)
                    .ConfigureAwait(false);

            RemoveEnvironments(widget, releaseDetails);

            AddEnvironments(widget, releaseDetails);
        }

        /// <summary>
        /// Find and remove environments that has been removed on VSTS.
        /// </summary>
        /// <param name="widget">VSTS Release widget</param>
        /// <param name="releaseDetails">VSTS release details, including environments.</param>
        private static void RemoveEnvironments(VSTSRelease_v1 widget, VSTSReleaseDetails releaseDetails)
        {
            if (widget == null || widget.Items == null)
                throw new InvalidOperationException();

            var environments = widget.Items
                .Where(k => !releaseDetails.Environments.Any(e => e.Name == k.Name))
                .ToList();

            foreach (var environment in environments)
            {
                Application.Current.Dispatcher.Invoke(() => widget.Remove(environment));
            }
        }

        //Add environments to the list.
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