using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
{
    public class AzureDevOpsWidget : Widget
    {
        protected const string Category = "Azure DevOps";

        public AzureDevOpsConnection Connection { get; set; }

        protected AzureDevOpsWidget(bool aggregate) : base(aggregate) { }
    }
}