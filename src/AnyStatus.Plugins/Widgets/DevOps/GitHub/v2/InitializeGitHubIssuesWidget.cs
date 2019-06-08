using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub.v2
{
    class InitializeGitHubIssuesWidget : RequestHandler<InitializeRequest<GitHubIssuesWidget>>
    {
        protected override void HandleCore(InitializeRequest<GitHubIssuesWidget> request)
        {
            request.DataContext.Clear();
        }
    }
}
