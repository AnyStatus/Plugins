using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

//todo: unautherized state when 401

//https://www.visualstudio.com/en-us/docs/integrate/api/rm/contracts#ReleaseStatus

namespace AnyStatus
{
    public class VSTSReleaseMonitor : IMonitor<VSTSRelease_v1>
    {
        [DebuggerStepThrough]
        public void Handle(VSTSRelease_v1 vstsRelease)
        {
            var client = new VstsClient
            {
                Connection = new VstsConnection
                {
                    Account = vstsRelease.Account,
                    Project = vstsRelease.Project,
                    UserName = vstsRelease.UserName,
                    Password = vstsRelease.Password,
                }
            };

            if (vstsRelease.DefinitionId == null)
            {
                var definition = client.GetReleaseDefinitionAsync(vstsRelease.ReleaseDefinitionName).Result;

                vstsRelease.DefinitionId = definition.Id;
            }

            var latestRelease = client.GetLatestReleaseAsync(vstsRelease.DefinitionId.Value).Result;

            var releaseDetails = client.GetReleaseDetailsAsync(latestRelease.Id).Result;

            RemoveEnvironments(vstsRelease, releaseDetails);

            AddEnvironments(vstsRelease, releaseDetails);
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