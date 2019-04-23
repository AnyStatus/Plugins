using AnyStatus.API;

namespace AnyStatus.Plugins.Demo
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Widget = new WindowsService();
        }

        public Widget Widget { get; set; }
    }
}
