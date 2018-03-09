using AnyStatus.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.Jenkins
{
    [TestClass]
    public class JenkinsViewTests
    {
        [TestMethod]
        public async Task JenkinsView_Should_NOT_DuplicateJobs_When_Refreshing()
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

            var handler = new JenkinsViewMonitor(logger);
            var request = HealthCheckRequest.Create(jenkinsView);

            await handler.Handle(request, CancellationToken.None);

            Assert.IsNotNull(jenkinsView.Items);
            Assert.IsTrue(jenkinsView.Items.Any());

            var count = jenkinsView.Items.Count;

            await handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(count, jenkinsView.Items.Count);
        }
    }
}
