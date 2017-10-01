using AnyStatus.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins Job")]
    [Description("Monitor the status of a Jenkins Jobs.")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsJob_v1 : Build, IJenkinsPlugin, IMonitored, ICanOpenInBrowser, ICanTriggerBuild, IReportProgress
    {
        private string _url;
        private int _progress;
        private bool _showProgress;

        public JenkinsJob_v1()
        {
            if (BuildParameters == null) BuildParameters = new List<NameValuePair>();
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins job URL address")]
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
        [Category("Jenkins")]
        [Editor(typeof(DataGridEditor), typeof(DataGridEditor))]
        [DisplayName("Parameters")]
        [Description("Optional. Specify the build parameters to use when triggering a new build. Parameters are case-sensitive.")]
        public List<NameValuePair> BuildParameters { get; set; }

        [PropertyOrder(50)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(60)]
        [Description("Indicates that the Jenkins CSRF protection is enabled.")]
        [Category("Jenkins")]
        public bool CSRF { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool HasBuildParameters
        {
            get
            {
                return BuildParameters != null && BuildParameters.Any();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool ShowProgress
        {
            get
            {
                return _showProgress;
            }
            set
            {
                _showProgress = value;
                OnPropertyChanged();
            }
        }

        public void ResetProgress()
        {
            ShowProgress = false;
            Progress = 0;
        }

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.EndsWith("/") ? str : str + "/";
        }
    }
}