using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.GitHub
{
    public class OpenGitHubIssueWebPage : OpenWebPage<GitHubIssueWidget>
    {
        public OpenGitHubIssueWebPage(IProcessStarter ps) : base(ps) { }
    }
}
