using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace AnyStatus.Monitors.Tests
{
    [TestClass]
    public class BatchScriptTests
    {
        private static TestContext _testContext;

        IProcessStarter _processStarter = Substitute.For<IProcessStarter>();

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\BatchScript.cmd")]
        public void Should_Execute_BatchScript()
        {
            var request = new BatchFile
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "BatchScript.cmd")
            };

            var handler = new BatchFileMonitor(_processStarter);

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Should_Throw_When_FileNotFound()
        {
            var request = new BatchFile
            {
                FileName = string.Empty
            };

            var handler = new BatchFileMonitor(_processStarter);

            handler.Handle(request);
        }
    }
}
