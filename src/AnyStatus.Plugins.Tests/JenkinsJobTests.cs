using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace AnyStatus.Plugins.Tests
{
    [TestClass]
    public class JenkinsJobTests
    {
        private const string Category = "Integration";
        [TestMethod]
        [TestCategory(Category)]
        public void JenkinsJobs_Monitor()
        {
            var jenkinsJobPlugin = new JenkinsJob_v1
            {
                Name = "Jenkins Core",
                IgnoreSslErrors = true,
                URL = @"https://builds.apache.org/job/logging-log4net/job/master/",
            };

            var logger = Substitute.For<ILogger>();

            var jenkinsClient = new JenkinsClient(logger);

            var jenkins = new JenkinsJobMonitor(jenkinsClient);

            jenkins.Handle(jenkinsJobPlugin);

            Assert.AreNotEqual(State.None, jenkinsJobPlugin.State, "Plugin state is None.");
            Assert.AreNotEqual(State.Unknown, jenkinsJobPlugin.State, "Plugin state is Unknown.");
            Assert.AreNotEqual(State.Error, jenkinsJobPlugin.State, "Plugin state is Error.");
        }

        [TestMethod]
        [TestCategory(Category)]
        //[ExpectedException(typeof(HttpRequestException))]
        public async Task JenkinsJob_TriggerAsync_Should_Fail_When_CrumbIsInvalid()
        {
            try
            {
                var logger = Substitute.For<ILogger>();
                var dialogService = Substitute.For<IDialogService>();
                var jenkinsClient = new JenkinsClient(logger);

                dialogService.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
                    .Returns(MessageBoxResult.Yes);

                var jenkinsJob = new JenkinsJob_v1
                {
                    Name = "Jenkins Core",
                    IgnoreSslErrors = true,
                    CSRF = false,
                    UserName = "a763847",
                    ApiToken = "8e38a9e38042711840782afe22ae535c",
                    URL = @"http://build-git:8080/view/PHA/job/PHA_18.1_Full/",
                };

                var trigger = new TriggerJenkinsJob(dialogService, logger, jenkinsClient);

                await trigger.HandleAsync(jenkinsJob);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
