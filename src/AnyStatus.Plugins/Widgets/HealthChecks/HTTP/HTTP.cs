using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("HTTP(S)")]
    [DisplayColumn("Health Checks")]
    [Description("Check web server HTTP response code")]
    public class HttpStatus : Widget, IHealthCheck, ISchedulable, IWebPage
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        //backward compatibility
        [Required]
        [Category("HTTP")]
        [PropertyOrder(10)]
        [DisplayName("URL")]
        public string Url { get; set; }

        [Browsable(false)]
        public string URL => Url;

        [PropertyOrder(20)]
        [Category("HTTP")]
        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [PropertyOrder(30)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(40)]
        [DisplayName("Use Default Credentials")]
        public bool UseDefaultCredentials { get; set; }

        public bool CanOpenInBrowser()
        {
            return State != State.None && State != State.Error;
        }
    }
}