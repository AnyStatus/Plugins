using AnyStatus.API;

namespace AnyStatus.Plugins.Demo
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Widget = new GitHubIssueV1();
        }

        public Widget Widget { get; set; }
    }
}
