using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub.v2
{
    public class OpenGitHubIssueWebPage : OpenWebPage<GitHubIssueWidget>
    {
        public OpenGitHubIssueWebPage(IProcessStarter ps) : base(ps) { }
    }

    public class OpenGitHubIssuesWebPage : OpenWebPage<GitHubIssuesWidget>
    {
        public OpenGitHubIssuesWebPage(IProcessStarter ps) : base(ps) { }
    }
}
