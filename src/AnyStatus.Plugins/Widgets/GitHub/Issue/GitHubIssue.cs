using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("GitHub Issue")]
    [Description("GitHub issue status")]
    public class GitHubIssue : Widget, IHealthCheck, ISchedulable, IWebPage
    {
        [Required]
        [Category("GitHub")]
        [Description("GitHub repository owner (user name).")]
        public string Owner { get; set; }

        [Required]
        [Category("GitHub")]
        [Description("GitHub repository name.")]
        public string Repository { get; set; }

        [Required]
        [Category("GitHub")]
        [DisplayName("Issue Number")]
        [Description("GitHub issue number.")]
        public int IssueNumber { get; set; }

        public string URL => $"https://api.github.com/repos/{Owner}/{Repository}/issues/{IssueNumber}";

        //public bool CanOpenInBrowser()
        //{
        //    return State != State.None && State != State.Error;
        //}
    }
}