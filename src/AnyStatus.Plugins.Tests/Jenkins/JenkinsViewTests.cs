using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;

namespace AnyStatus.Plugins.Tests.Jenkins
{
    [TestClass]
    public class JenkinsViewTests
    {
        [TestMethod]
        public void JenkinsView_Should_NOT_DuplicateJobs_When_Refreshing()
        {
            var logger = Substitute.For<ILogger>();

            var folder = new Folder();

            var jenkinsView = new JenkinsView_v1
            {
                Name = "Jenkins View",
                Interval = 0,
                URL = @"https://jenkins.mono-project.com/view/Urho/",
                IgnoreSslErrors = true
            };

            folder.Add(jenkinsView);

            var jenkinsHandler = new JenkinsViewMonitor(new JenkinsClient(logger));

            jenkinsHandler.Handle(jenkinsView);

            Assert.IsNotNull(jenkinsView.Items);
            Assert.IsTrue(jenkinsView.Items.Any());

            var count = jenkinsView.Items.Count;

            jenkinsHandler.Handle(jenkinsView);

            Assert.AreEqual(count, jenkinsView.Items.Count);
        }
    }
}
