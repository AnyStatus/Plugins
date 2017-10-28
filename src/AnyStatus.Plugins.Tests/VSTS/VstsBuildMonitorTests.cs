using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [Ignore]
        [TestMethod]
        public void TestMethod1()
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

            var monitor = new VSTSBuildMonitor();

            monitor.Handle(build);

            Assert.AreEqual(State.Ok, build.State);
        }
    }
}
