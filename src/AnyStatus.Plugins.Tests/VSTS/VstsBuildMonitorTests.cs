using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var build = new VSTSBuild_v1
            {
                Account = "production",
                Project = "AnyStatus",
                DefinitionName = "AnyStatus.CI.Test",
                Name = "Test",
                UserName = string.Empty,
                Password = "ddmqi7d2id6so5mjvosiyexzexacwwixa7dwnxfmlg6qd4xdwkfa"
            };

            var monitor = new VSTSBuildMonitor();

            monitor.Handle(build);

            Assert.AreEqual(State.Ok, build.State);
        }
    }
}
