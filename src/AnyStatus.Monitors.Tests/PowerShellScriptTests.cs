using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace AnyStatus.Monitors.Tests
{
    [TestClass]
    public class PowerShellScriptTests
    {
        private static TestContext _testContext;

        IProcessStarter _processStarter = Substitute.For<IProcessStarter>();

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\PowerShell.ps1")]
        public void Should_Execute_PowerShellScript()
        {

            var request = new PowerShellScript
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "PowerShell.ps1")
            };

            var handler = new PowerShellScriptMonitor(_processStarter);

            handler.Handle(request);

            Assert.AreSame(State.Ok, request.State);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Should_Throw_When_FileNotFound()
        {
            var request = new PowerShellScript
            {
                FileName = string.Empty
            };

            var handler = new PowerShellScriptMonitor(_processStarter);

            handler.Handle(request);
        }
    }
}
