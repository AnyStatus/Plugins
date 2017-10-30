using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class VstsBuildTests
    {
        [TestMethod]
        public void OpenInBrowserTest()
        {
            var processstarter = Substitute.For<IProcessStarter>();

            var vstsBuild = new VSTSBuild_v1
            {
                Account = "account",
                Project = "project",
                DefinitionId = 1
            };

            var openVstsBuildPage = new OpenVstsBuildPage(processstarter);

            openVstsBuildPage.Handle(vstsBuild);

            var expected = "https://account.visualstudio.com/project/_build/index?definitionId=1&_a=completed";

            processstarter.Received().Start(expected);
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
