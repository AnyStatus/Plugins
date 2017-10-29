using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins View (Preview)")]
    [Description("Shows the status of a Jenkins view or multi-branch (pipeline).")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsView_v1 : Plugin, IJenkinsPlugin, IMonitored, ICanOpenInBrowser
    {
        private string _url;

        public JenkinsView_v1() : base(aggregate: true) { }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins view job URL address")]
        public string URL
        {
            get { return _url; }
            set { _url = EnsureEndsWithSlash(value); }
        }

        [PropertyOrder(20)]
        [Category("Jenkins")]
        [DisplayName("User Name")]
        [Description("The Jenkins user name (optional)")]
        public string UserName { get; set; }

        [PropertyOrder(30)]
        [Category("Jenkins")]
        [DisplayName("API Token")]
        [Description("The Jenkins API token (optional). The API token is available in your personal configuration page. Click your name on the top right corner on every page, then click “Configure” to see your API token.")]
        public string ApiToken { get; set; }

        [PropertyOrder(40)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(50)]
        [Description("Indicates that the Jenkins CSRF protection is enabled.")]
        [Category("Jenkins")]
        public bool CSRF { get; set; }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ?
                    str :
                    str.EndsWith("/") ? str : str + "/";
        }

        public bool CanOpenInBrowser()
        {
            return State != State.Error;
        }
    }
}