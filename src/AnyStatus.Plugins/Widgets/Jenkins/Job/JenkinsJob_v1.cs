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
    public class JenkinsJob_v1 : Build, IJenkins, IHealthCheck, ISchedulable, IWebPage, IStartable, IReportProgress
    {
        private string _url;
        private int _progress;
        private bool _showProgress;
        private bool _isParameterized;

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
        [Description("Optional. Enter your Jenkins user name ")]
        public string UserName { get; set; }

        [PropertyOrder(30)]
        [Category("Jenkins")]
        [DisplayName("API Token")]
        [Description("Optional. Enter your Jenkins API Token. The API token is available in your personal configuration page. Click your name on the top right corner on every page, then click “Configure” to see your API token.")]
        public string ApiToken { get; set; }

        [PropertyOrder(40)]
        [Description("Check if your Jenkins uses the \"Prevent Cross Site Request Forgery exploits\" security option.")]
        [Category("Jenkins")]
        public bool CSRF { get; set; }

        [PropertyOrder(50)]
        [Category("Jenkins")]
        [DisplayName("Parameterized Build")]
        //[RefreshProperties(RefreshProperties.All)] <- wpftoolkit bug - description text disapears
        [Description("Check if your build is parameterized. You can find out in the configuration page of your Jenkins job.")]
        public bool IsParameterized
        {
            get
            {
                return _isParameterized;
            }
            set
            {
                _isParameterized = value;
            }
        }

        [PropertyOrder(60)]
        [Category("Jenkins")]
        [DisplayName("Parameters")]
        [Description("Optional (case-sensitive).")]
        [Editor(typeof(DataGridEditor), typeof(DataGridEditor))]
        public List<NameValuePair> BuildParameters { get; set; }

        [PropertyOrder(70)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

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

        //public bool CanOpenInBrowser()
        //{
        //    return State != State.None && State != State.Error;
        //}

        //public bool CanTriggerBuild()
        //{
        //    return State != State.None && State != State.Error;
        //}
    }
}