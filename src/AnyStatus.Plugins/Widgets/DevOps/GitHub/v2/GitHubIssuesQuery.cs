using AnyStatus.API;
using AnyStatus.API.Common.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub.v2
{
    public class GitHubIssuesQuery : IMetricQuery<GitHubIssuesWidget>
    {
        private readonly IUiAction _uiAction;

        public GitHubIssuesQuery(IUiAction uiAction)
        {
            _uiAction = uiAction;
        }

        public async Task Handle(MetricQueryRequest<GitHubIssuesWidget> request, CancellationToken cancellationToken)
        {
            var issues = await new GitHubApi().GetIssuesAsync(request.DataContext.Owner, request.DataContext.Repository).ConfigureAwait(false);

            request.DataContext.Value = issues.Count();

            var synchronizer = new CollectionSynchronizer<GitHubIssue, Item>
            {
                Compare = (issue, item) => item is GitHubIssueWidget issueWidget && issue.Number == issueWidget.Issue,
                Add = issue => request.DataContext.Add(new GitHubIssueWidget
                {
                    Name = issue.Title,
                    URL = issue.HtmlUrl,
                    Owner = request.DataContext.Owner,
                    Repository = request.DataContext.Repository,
                    Issue = issue.Number,
                    State = State.Ok
                }),
                Update = (issue, item) => item.Name = issue.Title,
                Remove = item => request.DataContext.Remove(item)
            };

            _uiAction.Invoke(() => synchronizer.Synchronize(issues.ToList(), request.DataContext.Items));

            request.DataContext.State = State.Ok;
        }
    }
}
