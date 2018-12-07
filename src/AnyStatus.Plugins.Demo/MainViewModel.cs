using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets;

namespace AnyStatus.Plugins.Demo
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Widget = new AzureDevOpsBuildWidget();
        }

        public Widget Widget { get; set; }
    }
}
