using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.AppVeyor
{
    [TestClass]
    public class AppVeyorTests
    {
        [TestMethod]
        public async Task AppVeyor_DefaultBranch()
        {
            var appVeyorBuild = new AppVeyorBuild
            {
                AccountName = "AnyStatus",
                ProjectSlug = "api"
            };

            var appVeyorBuildMonitor = new AppVeyorBuildMonitor();

            var request = HealthCheckRequest.Create(appVeyorBuild);

            await appVeyorBuildMonitor.Handle(request, CancellationToken.None);

            Assert.AreNotEqual(State.None, appVeyorBuild.State, "State is None.");
            Assert.AreNotEqual(State.Error, appVeyorBuild.State, "State is Error.");
        }

        [TestMethod]
        public async Task AppVeyor_SpecifiedBranch()
        {
            var appVeyorBuild = new AppVeyorBuild
            {
                AccountName = "AnyStatus",
                ProjectSlug = "api",
                SourceControlBranch = "master"
            };

            var appVeyorBuildMonitor = new AppVeyorBuildMonitor();

            var request = HealthCheckRequest.Create(appVeyorBuild);

            await appVeyorBuildMonitor.Handle(request, CancellationToken.None);

            Assert.AreNotEqual(State.None, appVeyorBuild.State, "State is None.");
            Assert.AreNotEqual(State.Error, appVeyorBuild.State, "State is Error.");
        }
    }
}