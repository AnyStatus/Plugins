using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class OpenInBrowserTests
    {
        [TestMethod]
        public async Task OpenGitHubIssueInBrowser()
        {
            var gitHubIssue = new GitHubIssue
            {
                IssueNumber = 1,
                Name = "name",
                Owner = "owner",
                Repository = "repository"
            };

            var processStarter = Substitute.For<IProcessStarter>();

            var handler = new OpenGitHubIssueWebPage(processStarter);

            var request = OpenWebPageRequest.Create(gitHubIssue);

            await handler.Handle(request, CancellationToken.None);

            processStarter.Received().Start(Arg.Any<string>());
        }
    }
}