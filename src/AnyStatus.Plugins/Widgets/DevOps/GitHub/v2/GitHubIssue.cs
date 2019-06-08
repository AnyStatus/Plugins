namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    public class GitHubIssue
    {
        public string Title { get; set; }

        public string HtmlUrl { get; internal set; }

        public int Number { get; internal set; }
    }
}