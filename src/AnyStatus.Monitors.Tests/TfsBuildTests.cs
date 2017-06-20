using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Monitors.Tests
{
    [TestClass]
    public class TfsBuildTests
    {
        [TestMethod]
        public void Clone_Should_Reset_BuildDefinitionId()
        {
            var tfsBuild = new TfsBuild
            {
                BuildDefinitionId = 1
            };

            var clone = tfsBuild.Clone() as TfsBuild;

            Assert.AreEqual(0, clone.BuildDefinitionId);
        }
    }
}
