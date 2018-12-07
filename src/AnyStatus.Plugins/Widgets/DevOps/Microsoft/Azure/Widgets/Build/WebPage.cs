using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets;

namespace AnyStatus
{
    public class AzureDevOpsBuildWebPage : OpenWebPage<AzureDevOpsBuildWidget>
    {
        public AzureDevOpsBuildWebPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}