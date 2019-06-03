using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class PowerShellScriptTests
    {
        private static TestContext _testContext;

        private IProcessStarter _processStarter = Substitute.For<IProcessStarter>();

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        [DeploymentItem(@"Scripts\PowerShell.ps1")]
        public void Should_Execute_PowerShellScript()
        {
            var powershell = new PowerShellScript
            {
                FileName = Path.Combine(_testContext.TestRunDirectory, "Out", "PowerShell.ps1")
            };

            var handler = new PowerShellRunner(_processStarter);

            var request = HealthCheckRequest.Create(powershell);

            handler.Handle(request, CancellationToken.None);

            Assert.AreSame(State.Ok, request.DataContext.State);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Should_Throw_When_FileNotFound()
        {
            var powershell = new PowerShellScript
            {
                FileName = string.Empty
            };

            var handler = new PowerShellRunner(_processStarter);

            var request = HealthCheckRequest.Create(powershell);

            handler.Handle(request, CancellationToken.None);
        }
    }
}