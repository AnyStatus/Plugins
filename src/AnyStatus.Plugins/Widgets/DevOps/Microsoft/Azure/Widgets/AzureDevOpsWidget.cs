using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
{
    public abstract class AzureDevOpsWidget : Widget
    {
        protected const string Category = "Azure DevOps";

        public AzureDevOpsConnection Connection { get; set; }

        public AzureDevOpsWidget() : this(false) { }

        protected AzureDevOpsWidget(bool aggregate) : base(aggregate) { }
    }
}