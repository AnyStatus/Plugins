using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class BatchScriptTests
    {
        private static TestContext _testContext;

        private IProcessStarter _processStarter = Substitute.For<IProcessStarter>();

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\BatchScript.cmd")]
        public void Should_Execute_BatchScript()
        {
            var batchFile = new BatchFile
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "BatchScript.cmd")
            };

            var handler = new BatchFileRunner(_processStarter);

            var request = HealthCheckRequest.Create(batchFile);

            handler.Handle(request, CancellationToken.None);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Should_Throw_When_FileNotFound()
        {
            var batchFile = new BatchFile
            {
                FileName = string.Empty
            };

            var handler = new BatchFileRunner(_processStarter);

            var request = HealthCheckRequest.Create(batchFile);

            handler.Handle(request, CancellationToken.None);
        }
    }
}