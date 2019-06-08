using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    /// <summary>
    /// Single GitHub Issue Widget.
    /// </summary>
    [Browsable(false)]
    [DisplayColumn("DevOps")]
    [DisplayName("GitHub Issue")]
    public class GitHubIssueWidget : Widget, IWebPage
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
        [Description("Optional. The GitHub issue number.")]
        public int Issue { get; set; }

        [ReadOnly(true)]
        public string URL { get; set; }
    }
}
