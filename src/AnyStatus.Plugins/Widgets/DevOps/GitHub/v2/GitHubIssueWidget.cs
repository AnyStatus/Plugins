using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.DevOps.GitHub
{
    /// <summary>
    /// Single GitHub Issue Widget.
    /// </summary>
    //[ReadOnly(true)]
    [Browsable(false)]
    [DisplayColumn("DevOps")]
    [DisplayName("GitHub Issue (Preview)")]
    [Description("GitHub repository issue.")]
    public class GitHubIssueWidget : Widget, IWebPage
    {
        [ReadOnly(true)]
        [Required]
        [PropertyOrder(1)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository owner name.")]
        public string Owner { get; set; }

        [ReadOnly(true)]
        [Required]
        [PropertyOrder(2)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository name.")]
        public string Repository { get; set; }

        [ReadOnly(true)]
        [PropertyOrder(3)]
        [Category("GitHub")]
        [DisplayName("Issue")]
        [Description("Optional. The GitHub issue number.")]
        public int Issue { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL { get; set; }
    }
}
