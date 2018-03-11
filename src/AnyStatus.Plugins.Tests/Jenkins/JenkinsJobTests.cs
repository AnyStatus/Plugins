using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class JenkinsJobTests
    {
        private const string Category = "Integration";

#if !DEBUG
        [Ignore]
#endif

        [TestMethod]
        [TestCategory(Category)]
        public async Task JenkinsJobMonitor()
        {
            var jenkinsJob = new JenkinsJob_v1
            {
                Name = "Jenkins Core",
                IgnoreSslErrors = true,
                URL = @"https://builds.apache.org/job/logging-log4net/job/master/",
            };

            var logger = Substitute.For<ILogger>();
            var handler = new JenkinsJobMonitor(logger);
            var request = HealthCheckRequest.Create(jenkinsJob);

            await handler.Handle(request, CancellationToken.None);

            if (jenkinsJob.State == State.None ||
                jenkinsJob.State == State.Error ||
                jenkinsJob.State == State.Unknown)
            {
                Assert.Fail("Invalid widget state");
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory(Category)]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task JenkinsJob_TriggerAsync_Should_Fail_When_CrumbIsInvalid()
        {
            var logger = Substitute.For<ILogger>();
            var dialogService = Substitute.For<IDialogService>();

            dialogService.ShowDialog(Arg.Any<ConfirmationDialog>()).Returns(DialogResult.Yes);

            var jenkinsJob = new JenkinsJob_v1
            {
                Name = "Jenkins Core",
                IgnoreSslErrors = true,
                CSRF = true,
                URL = @"https://ci.jenkins-ci.org/job/Core/job/jenkins/job/master/",
            };

            var trigger = new StartJenkinsJob(dialogService, logger);

            //await trigger.Handle(jenkinsJob);
        }
    }
}