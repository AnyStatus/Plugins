using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class VstsBuildTests
    {
        [TestMethod]
        public async Task OpenVstsBuildWebPage()
        {
            var ps = Substitute.For<IProcessStarter>();

            var build = new VSTSBuild_v1
            {
                Account = "account",
                Project = "project",
                DefinitionId = 1
            };

            var request = OpenWebPageRequest.Create(build);

            var handler = new OpenVstsBuildPage(ps);

            await handler.Handle(request, CancellationToken.None);

            var expected = "https://account.visualstudio.com/project/_build/index?definitionId=1&_a=completed";

            ps.Received().Start(expected);
        }

        [Ignore]
        [TestMethod]
        public void VstsBuildMonitorTest()
        {
            var build = new VSTSBuild_v1
            {
                Account = "production",
                Project = "Demo",
                DefinitionName = "Demo-CI",
                Name = "Test",
                UserName = string.Empty,
                Password = "personal access token"
            };

            var monitor = new VstsBuildMonitor();

            monitor.Handle(build);

            Assert.AreEqual(State.Ok, build.State);
        }
    }
}