using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

// replaced by JenkinsJob_v1
namespace AnyStatus
{
    [Browsable(false)]
    [Obsolete("Renamed to JenkinsView")]
    public class JenkinsMultiJob : Plugin
    {
        private string url;

        public JenkinsMultiJob() : base(aggregate: true)
        {
            State = State.Canceled;
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins multibranch job URL address")]
        public string Url
        {
            get { return this.url; }
            set { this.url = EnsureEndsWithSlash(value); }
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

        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ?
                    str :
                    str.EndsWith("/") ? str : str + "/";
        }
    }
}
