using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    public class OpenGitHubIssuesWebPage : OpenWebPage<GitHubIssuesWidget>
    {
        public OpenGitHubIssuesWebPage(IProcessStarter ps) : base(ps) { }
    }
}
