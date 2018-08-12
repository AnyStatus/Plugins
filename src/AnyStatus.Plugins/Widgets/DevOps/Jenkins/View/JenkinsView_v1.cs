using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins View")]
    [Description("Jenkins view and multi-branch (pipeline) status.")]
    [DisplayColumn("DevOps")]
    public class JenkinsView_v1 : Widget, IJenkins, IHealthCheck, ISchedulable, IWebPage
    {
        private const string Category = "Jenkins";

        private string _url;

        public JenkinsView_v1() : base(aggregate: true)
        {
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [Description("Jenkins view job URL address")]
        public string URL
        {
            get => _url;
            set => _url = EnsureEndsWithSlash(value);
        }

        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Ignore SSL errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("The Jenkins user name (optional)")]
        public string UserName { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("API Token")]
        [Description("The Jenkins API token (optional). The API token is available in your personal configuration page. Click your name on the top right corner on every page, then click “Configure” to see your API token.")]
        public string ApiToken { get; set; }

        [PropertyOrder(50)]
        [Category(Category)]
        [Description("Check if Jenkins CSRF protection is enabled.")]
        public bool CSRF { get; set; }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : str.EndsWith("/") ? str : str + "/";
        }
    }
}