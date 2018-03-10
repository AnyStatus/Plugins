using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.VSTS
{
    [TestClass]
    public class VstsReleaseTests
    {
        [TestMethod]
        public async Task OpenVstsReleaseWebPage()
        {
            var ps = Substitute.For<IProcessStarter>();

            var release = new VSTSRelease_v1
            {
                Account = "account",
                Project = "project",
                DefinitionId = 1
            };

            var request = OpenWebPageRequest.Create(release);

            var handler = new OpenVstsReleasePage(ps);

            await handler.Handle(request, CancellationToken.None);

            var expected = "https://account.visualstudio.com/project/_release?definitionId=1&_a=releases";

            ps.Received().Start(expected);
        }
    }
}