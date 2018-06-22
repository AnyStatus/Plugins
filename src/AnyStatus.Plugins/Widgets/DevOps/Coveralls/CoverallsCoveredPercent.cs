using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Coveralls")]
    [DisplayColumn("DevOps")]
    [Description("Coveralls code coverage (covered percentage).")]
    public class CoverallsCoveredPercent : MetricValue, ISchedulable, IWebPage
    {
        private const string Category = "Coveralls";

        public CoverallsCoveredPercent()
        {
            Threshold = 80;
        }

        //backward compatibility
        [Url]
        [Required]
        [DisplayName("URL")]
        [PropertyOrder(10)]
        [Category(Category)]
        [Description("Coveralls repository URL address. For example: https://coveralls.io/github/AlonAm/AnyStatus")]
        public string Url { get; set; }

        [Browsable(false)]
        public string URL => Url;

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [Description("Failed threshold.")]
        public int Threshold { get; set; }
    }
}