using AnyStatus.API;

namespace AnyStatus
{
    public class OpenGitHubIssueWebPage : OpenWebPage<GitHubIssue>
    {
        public OpenGitHubIssueWebPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}