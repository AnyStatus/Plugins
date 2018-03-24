using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.TeamCity
{
    [TestClass]
    public class TeamCityTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task TeamCityBuildMonitorTest()
        {
            var build = new TeamCityBuild
            {
                GuestUser = true,
                Url = "https://teamcity.jetbrains.com",
                BuildTypeId = "OpenSourceProjects_Kaxb_Build",
            };

            var request = HealthCheckRequest.Create(build);
            var handler = new TeamCityBuildStatus();

            await handler.Handle(request, CancellationToken.None);

            Assert.AreNotEqual(State.None, request.DataContext.State);
            Assert.AreNotEqual(State.Error, request.DataContext.State);
        }
    }
}