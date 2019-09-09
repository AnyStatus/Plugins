using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.GitHub
{
    class InitializeGitHubIssuesWidget : RequestHandler<InitializeRequest<GitHubIssuesWidget>>
    {
        protected override void HandleCore(InitializeRequest<GitHubIssuesWidget> request)
        {
            request.DataContext.Clear();
        }
    }
}
