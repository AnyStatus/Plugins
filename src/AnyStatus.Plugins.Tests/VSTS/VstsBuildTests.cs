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

            const string expected = "https://account.visualstudio.com/project/_build/index?definitionId=1&_a=completed";

            ps.Received().Start(expected);
        }

        [TestMethod]
        public async Task OpenVstsBuildWebPageWithSpacesInProjectName()
        {
            var ps = Substitute.For<IProcessStarter>();

            var build = new VSTSBuild_v1
            {
                Account = "account",
                Project = "project with spaces",
                DefinitionId = 1
            };

            var request = OpenWebPageRequest.Create(build);

            var handler = new OpenVstsBuildPage(ps);

            await handler.Handle(request, CancellationToken.None);

            const string expected = "https://account.visualstudio.com/project%20with%20spaces/_build/index?definitionId=1&_a=completed";

            ps.Received().Start(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(VstsClientException))]
        public async Task VstsBuildHealthCheckTest()
        {
            var logger = Substitute.For<ILogger>();

            var build = new VSTSBuild_v1
            {
                Account = "production",
                Project = "Demo",
                DefinitionName = "Demo-CI",
                Name = "Test",
                UserName = string.Empty,
                Password = "personal access token"
            };

            var handler = new VstsBuildHealthCheck(logger);

            var request = HealthCheckRequest.Create(build);

            await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);
        }
    }
}