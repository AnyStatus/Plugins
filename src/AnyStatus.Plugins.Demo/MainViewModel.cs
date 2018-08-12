using AnyStatus.API;

namespace AnyStatus.Plugins.Demo
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Widget = new TeamCityBuild();
        }

        public Widget Widget { get; set; }
    }
}
