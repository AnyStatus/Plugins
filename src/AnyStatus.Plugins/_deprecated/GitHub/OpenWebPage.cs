using AnyStatus.API;

namespace AnyStatus
{
    public class OpenGitHubIssueWebPage : OpenWebPage<GitHubIssueV1>
    {
        public OpenGitHubIssueWebPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}