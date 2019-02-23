using AnyStatus.API;

namespace AnyStatus.Plugins.Demo
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Widget = new CpuUsage();
        }

        public Widget Widget { get; set; }
    }
}
