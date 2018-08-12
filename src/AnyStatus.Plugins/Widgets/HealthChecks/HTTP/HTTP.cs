using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("HTTP")]
    [DisplayColumn("Health Checks")]
    [Description("HTTP monitoring enables you to test the availability of a web page.")]
    public class HttpStatus : Widget, IHealthCheck, ISchedulable, IWebPage
    {
        private const string Category = "HTTP";

        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        [Required]
        [PropertyOrder(10)]
        [DisplayName("URL")]
        [Category(Category)]
        public string Url { get; set; }

        [Browsable(false)]
        string IWebPage.URL => Url; //backward compatibility

        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("Ignore SSL errors")]
        public bool IgnoreSslErrors { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Use Default Credentials")]
        public bool UseDefaultCredentials { get; set; }
    }
}