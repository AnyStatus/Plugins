//using AnyStatus.API;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
//{
//    public class GitHubIssueQuery : ICheckHealth<GitHubIssueWidget>
//    {
//        public async Task Handle(HealthCheckRequest<GitHubIssueWidget> request, CancellationToken cancellationToken)
//        {
//            var issue = await new GitHubApi().GetIssueAsync(
//                request.DataContext.Owner, request.DataContext.Repository, request.DataContext.Issue).ConfigureAwait(false);

//            request.DataContext.Name = issue.Title;
//            request.DataContext.State = issue.State;
//        }
//    }
//}
