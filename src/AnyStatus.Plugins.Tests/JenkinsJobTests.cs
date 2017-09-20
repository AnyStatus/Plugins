using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
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

            var jenkins = new JenkinsJobMonitor();

            jenkins.Handle(jenkinsJobPlugin);

            Assert.AreNotEqual(State.None, jenkinsJobPlugin.State, "Plugin state is None.");
            Assert.AreNotEqual(State.Unknown, jenkinsJobPlugin.State, "Plugin state is Unknown.");
            Assert.AreNotEqual(State.Error, jenkinsJobPlugin.State, "Plugin state is Error.");
        }

        [TestMethod]
        [TestCategory(Category)]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task JenkinsJobs_TriggerAsync()
        {
            var logger = Substitute.For<ILogger>();
            var dialogService = Substitute.For<IDialogService>();

            dialogService.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>(), Arg.Any<MessageBoxImage>())
                .Returns(MessageBoxResult.Yes);

            var jenkinsJob = new JenkinsJob_v1
            {
                Name = "Jenkins Core",
                IgnoreSslErrors = true,
                URL = @"https://ci.jenkins-ci.org/job/Core/job/jenkins/job/master/",
            };

            var trigger = new TriggerJenkinsJob(dialogService, logger);

            await trigger.HandleAsync(jenkinsJob);
        }
    }
}
