using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Plugins.Tests.AppVeyor
{
    [TestClass]
    public class AppVeyorTests
    {
        [TestMethod]
        public void AppVeyor_DefaultBranch()
        {
            var appVeyorBuild = new AppVeyorBuild
            {
                AccountName = "AnyStatus",
                ProjectSlug = "api"
            };

            var appVeyorBuildMonitor = new AppVeyorBuildMonitor();

            appVeyorBuildMonitor.Handle(appVeyorBuild);

            Assert.AreNotEqual(State.None, appVeyorBuild.State, "State is None.");
            Assert.AreNotEqual(State.Error, appVeyorBuild.State, "State is Error.");
        }

        [TestMethod]
        public void AppVeyor_SpecifiedBranch()
        {
            var appVeyorBuild = new AppVeyorBuild
            {
                AccountName = "AnyStatus",
                ProjectSlug = "api",
                SourceControlBranch = "master"
            };

            var appVeyorBuildMonitor = new AppVeyorBuildMonitor();

            appVeyorBuildMonitor.Handle(appVeyorBuild);

            Assert.AreNotEqual(State.None, appVeyorBuild.State, "State is None.");
            Assert.AreNotEqual(State.Error, appVeyorBuild.State, "State is Error.");
        }
    }
}