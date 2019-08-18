using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("GitHub")]
    [DisplayName("GitHub Issue")]
    [XmlType(TypeName = "GitHubIssue")]
    public class GitHubIssueV1 : Widget, IHealthCheck, ISchedulable, IWebPage
    {
        [Required]
        [PropertyOrder(1)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository owner name.")]
        public string Owner { get; set; }

        [Required]
        [PropertyOrder(2)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository name.")]
        public string Repository { get; set; }

        [PropertyOrder(3)]
        [Category("GitHub")]
        [DisplayName("Issue")]
        [Description("Optional. The GitHub issue id.")]
        public int IssueNumber { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://api.github.com/repos/{Uri.EscapeDataString(Owner)}/{Uri.EscapeDataString(Repository)}/issues/{IssueNumber}";
    }
}