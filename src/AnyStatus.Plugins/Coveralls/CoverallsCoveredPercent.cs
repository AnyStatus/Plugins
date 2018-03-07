using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Coveralls")]
    [DisplayColumn("Continuous Integration")]
    [Description("Coveralls covered code percentage")]
    public class CoverallsCoveredPercent : Metric, ISchedulable, IWebPage
    {
        private const string Category = "Coveralls";

        public CoverallsCoveredPercent()
        {
            Threshold = 80;
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [Description("Coveralls repository URL address. For example: https://coveralls.io/github/AlonAm/AnyStatus")]
        public string URL { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [Description("Failed threshold.")]
        public int Threshold { get; set; }
    }
}