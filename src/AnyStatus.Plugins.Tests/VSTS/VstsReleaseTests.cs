using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.VSTS.Release;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.VSTS
{
    [Ignore]
    [TestClass]
    public class VstsReleaseTests
    {
        private const string Account = "account name";
        private const string Project = "project name";
        private const string Token = "toekn";
        private const string ReleaseDefinition = "release definition name";

        [TestMethod]
        public async Task OpenVstsReleaseWebPage()
        {
            var ps = Substitute.For<IProcessStarter>();

            var release = new VSTSRelease_v1
            {
                Account = "account",
                Project = "project",
                ReleaseId = 1
            };

            var request = OpenWebPageRequest.Create(release);

            var handler = new OpenVstsReleasePage(ps);

            await handler.Handle(request, CancellationToken.None);

            var expected = "https://account.visualstudio.com/project/_release?definitionId=1&_a=releases";

            ps.Received().Start(expected);
        }

        [TestMethod]
        public async Task VstsReleaseHealthCheck()
        {
            var release = new VSTSRelease_v1
            {
                Password = Token,
                Account = Account,
                Project = Project,
                ReleaseDefinitionName = ReleaseDefinition,
            };

            var request = HealthCheckRequest.Create(release);

            var handler = new VSTSReleaseHealthCheck();

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreNotEqual(State.None, release.State);
        }

        [TestMethod]
        public async Task DeployVstsRelease()
        {
            var release = new VSTSRelease_v1
            {
                Account = Account,
                Project = Project,
                Password = Token,
                ReleaseId = 11
            };

            var env = new VSTSReleaseEnvironment
            {
                EnvironmentId = 1149
            };

            release.Add(env);

            var request = StartRequest.Create(env);

            var dlg = Substitute.For<IDialogService>();

            dlg.ShowDialog(Arg.Any<ConfirmationDialog>()).Returns(_ => DialogResult.Yes);

            var handler = new DeployVstsRelease(dlg, Substitute.For<ILogger>());

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(State.Queued, release.State);
        }
    }
}