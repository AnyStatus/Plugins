using AnyStatus.API;
using AnyStatus.API.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

//todo: unautherized state when 401
//https://www.visualstudio.com/en-us/docs/integrate/api/rm/contracts#ReleaseStatus

namespace AnyStatus
{
    public class VSTSReleaseHealthCheck : ICheckHealth<VSTSRelease_v1>
    {
        public async Task Handle(HealthCheckRequest<VSTSRelease_v1> request, CancellationToken cancellationToken)
        {
            var widget = request.DataContext; // note, Data Context is auto-validated by the framework.

            var client = new VstsClient(new VstsConnection());

            widget.MapTo(client.Connection);

            if (widget.DefinitionId == null)
            {
                var definition = await client
                    .GetReleaseDefinitionAsync(widget.ReleaseDefinitionName)
                        .ConfigureAwait(false);

                widget.DefinitionId = definition.Id;
            }

            var lastRelease = await client
                .GetLastReleaseAsync(widget.DefinitionId.Value)
                    .ConfigureAwait(false);

            var releaseDetails = await client
                .GetReleaseDetailsAsync(lastRelease.Id)
                    .ConfigureAwait(false);

            RemoveEnvironments(widget, releaseDetails);

            AddEnvironments(widget, releaseDetails);
        }

        private static void RemoveEnvironments(VSTSRelease_v1 widget, VSTSReleaseDetails release)
        {
            var removedEnvironments = widget.Items.Where(k => !release.Environments.Any(e => e.Name == k.Name)).ToList();

            removedEnvironments.ForEach(env => Application.Current.Dispatcher.Invoke(() => widget.Remove(env)));
        }

        private static void AddEnvironments(VSTSRelease_v1 widget, VSTSReleaseDetails release)
        {
            if (widget == null || widget.Items == null)
                throw new InvalidOperationException();

            foreach (var environment in release.Environments)
            {
                var newEnvironment = widget.Items.FirstOrDefault(i => i.Name == environment.Name);

                if (newEnvironment == null)
                {
                    newEnvironment = AddEnvironment(widget, environment);
                }

                newEnvironment.State = environment.State;
            }
        }

        private static VSTSReleaseEnvironment AddEnvironment(VSTSRelease_v1 widget, ReleaseEnvironment environment)
        {
            var newEnvironment = new VSTSReleaseEnvironment
            {
                Name = environment.Name,
                EnvironmentId = environment.Id
            };

            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() => widget.Add(newEnvironment));

            return newEnvironment;
        }
    }
}