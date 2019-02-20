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
    [Description("Jenkins job status.")]
    [DisplayColumn("DevOps")]
    public class JenkinsJob_v1 : Build, IJenkins, IHealthCheck, ISchedulable, IWebPage, IStartable, IReportProgress
    {
        private const string Category = "Jenkins";
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
        [Category(Category)]
        [Description("Jenkins job URL address")]
        public string URL
        {
            get { return _url; }
            set { _url = EnsureEndsWithSlash(value); }
        }

        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Ignore SSL errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("Optional. Enter your Jenkins user name ")]
        public string UserName { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("API Token")]
        [Description("Optional. Enter your Jenkins API Token. The API token is available in your personal configuration page. Click your name on the top right corner on every page, then click “Configure” to see your API token.")]
        public string ApiToken { get; set; }

        [PropertyOrder(50)]
        [Category(Category)]
        [Description("Check if your Jenkins uses the \"Prevent Cross Site Request Forgery exploits\" security option.")]
        public bool CSRF { get; set; }

        [PropertyOrder(60)]
        [Category(Category)]
        [DisplayName("Parameterized Build")]
        [RefreshProperties(RefreshProperties.All)]
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

                OnPropertyChanged();

                SetPropertyVisibility(nameof(BuildParameters), _isParameterized);
            }
        }

        [Browsable(false)]
        [PropertyOrder(70)]
        [Category(Category)]
        [DisplayName("Parameters")]
        [Description("Optional (case-sensitive).")]
        [Editor(typeof(DataGridEditor), typeof(DataGridEditor))]
        public List<NameValuePair> BuildParameters { get; set; }

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

        private static string EnsureEndsWithSlash(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.EndsWith("/") ? str : str + "/";
        }
    }
}