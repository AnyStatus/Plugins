using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("DevOps")]
    [DisplayName("GitHub Issue")]
    [Description("GitHub issue status.")]
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

        public string URL => $"https://api.github.com/repos/{Uri.EscapeDataString(Owner)}/{Uri.EscapeDataString(Repository)}/issues/{IssueNumber}";
    }
}