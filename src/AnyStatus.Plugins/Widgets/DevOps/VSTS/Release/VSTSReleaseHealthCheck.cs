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

            var client = new VstsClient();

            widget.MapTo(client);

            if (widget.ReleaseId == null)
            {
                var definition = await client
                    .GetReleaseDefinitionAsync(widget.ReleaseDefinitionName)
                        .ConfigureAwait(false);

                widget.ReleaseId = definition.Id;
            }

            var lastRelease = await client
                .GetLastReleaseAsync(widget.ReleaseId.Value)
                    .ConfigureAwait(false);

            var releaseDetails = await client
                .GetReleaseDetailsAsync(lastRelease.Id)
                    .ConfigureAwait(false);

            RemoveEnvironments(widget, releaseDetails);

            AddEnvironments(widget, releaseDetails);
        }

        private static void RemoveEnvironments(Widget widget, VSTSReleaseDetails release)
        {
            var removedEnvironments = widget.Items.Where(k => release.Environments.All(e => e.Name != k.Name)).ToList();

            removedEnvironments.ForEach(env => Application.Current.Dispatcher.Invoke(() => widget.Remove(env)));
        }

        private static void AddEnvironments(VSTSRelease_v1 widget, VSTSReleaseDetails release)
        {
            if (widget?.Items == null)
                throw new InvalidOperationException();

            foreach (var env in release.Environments)
            {
                var environment = widget.Items.FirstOrDefault(i => i.Name == env.Name) ?? AddEnvironment(widget, env);

                environment.State = env.State;
            }
        }

        private static VSTSReleaseEnvironment AddEnvironment(VSTSRelease_v1 release, ReleaseEnvironment environment)
        {
            if (!release.ReleaseId.HasValue)
                throw new InvalidOperationException("Release id was not set.");

            var newEnvironment = new VSTSReleaseEnvironment
            {
                Name = environment.Name,
                EnvironmentId = environment.Id,
            };

            var dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            dispatcher.Invoke(() => release.Add(newEnvironment));

            return newEnvironment;
        }
    }
}