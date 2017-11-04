using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Plugins.Tests.TeamCity
{
    [TestClass]
    public class TeamCityTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void TeamCityBuildMonitorTest()
        {
            var teamCityBuild = new TeamCityBuild
            {
                GuestUser = true,
                Url = "https://teamcity.jetbrains.com",
                BuildTypeId = "OpenSourceProjects_Kaxb_Build",
            };

            var monitor = new TeamCityBuildMonitor();

            monitor.Handle(teamCityBuild);

            Assert.AreNotEqual(State.None, teamCityBuild.State);
            Assert.AreNotEqual(State.Error, teamCityBuild.State);
        }
    }
}
