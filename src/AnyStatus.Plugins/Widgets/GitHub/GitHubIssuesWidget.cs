using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using AnyStatus.API;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.GitHub
{
    /// <summary>
    /// GitHub Issues List Widget.
    /// </summary>
    [DisplayColumn("GitHub")]
    [DisplayName("GitHub Issues")]
    [Description("GitHub repository issues list.")]
    public class GitHubIssuesWidget : Metric, IWebPage, ISchedulable, IInitializable
    {
        [Required]
        [PropertyOrder(1)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository owner user name.")]
        public string Owner { get; set; }

        [Required]
        [PropertyOrder(2)]
        [Category("GitHub")]
        [Description("Required. The GitHub repository name.")]
        public string Repository { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://github.com/{Uri.EscapeDataString(Owner)}/{Uri.EscapeDataString(Repository)}/issues";

        [XmlIgnore]
        [Browsable(false)]
        public bool IsInitialized { get; set; }
    }
}
