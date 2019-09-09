using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API;
using AnyStatus.API.Common.Services;
using AnyStatus.Plugins.Widgets.GitHub.API;

namespace AnyStatus.Plugins.Widgets.GitHub
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

            var list = issues.ToList();

            request.DataContext.Value = list.Count;

            var synchronizer = GetSynchronizer(request);

            _uiAction.Invoke(() => synchronizer.Synchronize(list, request.DataContext.Items));

            request.DataContext.State = State.Ok;
        }

        private static CollectionSynchronizer<GitHubIssue, Item> GetSynchronizer(MetricQueryRequest<GitHubIssuesWidget> request)
        {
            return new CollectionSynchronizer<GitHubIssue, Item>
            {
                Compare = (issue, item) => item is GitHubIssueWidget issueWidget && issue.Number == issueWidget.Issue,
                Remove = item => request.DataContext.Remove(item),
                Update = (issue, item) => item.Name = issue.Title,
                Add = issue => request.DataContext.Add(new GitHubIssueWidget
                {
                    Name = issue.Title,
                    URL = issue.HtmlUrl,
                    Owner = request.DataContext.Owner,
                    Repository = request.DataContext.Repository,
                    Issue = issue.Number,
                    State = State.Ok,
                    Interval = 0 //bypass scheduler
                })
            };
        }
    }
}
