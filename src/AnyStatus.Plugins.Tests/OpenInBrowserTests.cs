using AnyStatus.API;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AnyStatus.Integration.Tests
{
    [TestClass]
    public class OpenInBrowserTests
    {
        [TestMethod]
        public void OpenGitHubIssueInBrowser()
        {
            var gitHubIssue = new GitHubIssue
            {
                IssueNumber = 1,
                Name = "name",
                Owner = "owner",
                Repository = "repository"
            };

            var processStarter = Substitute.For<IProcessStarter>();

            var handler = new OpenGitHubIssueInBrowser(processStarter);

            handler.Handle(gitHubIssue);

            processStarter.Received().Start(Arg.Any<string>());
        }
    }
}
