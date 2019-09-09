using AnyStatus.API;
using AnyStatus.Plugins.Widgets.NuGet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Tests.Widgets.NuGet
{
    [TestClass]
    public class NuGetPackageWidgetTests
    {
        [TestMethod]
        public async Task NuGetPackageWidgetTest()
        {
            var widget = new NuGetPackageWidget
            {
                PackageId = "AnyStatus.API"
            };

            var handler = new NuGetPackageQuery();

            var request = MetricQueryRequest.Create(widget);

            await handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(State.Ok, widget.State);
            Assert.IsNotNull(widget.Value);
            Assert.IsNotNull(widget.Message);
        }
    }
}
