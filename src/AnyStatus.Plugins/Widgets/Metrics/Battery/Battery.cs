using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("Battery")]
    [XmlType(TypeName = "Battery_v1")]
    [Description("Display the battery status information.")]
    public class Battery : Metric, ISchedulable, IReportProgress
    {
        public Battery()
        {
            ShowProgress = true;
            Name = "Battery";
        }

        [DisplayName("Threshold")]
        [Description("Battery life percent threshold.")]

        public int BatteryLifePercentThreshold { get; set; }

        #region IReportProgress

        [DisplayName("Show Progress")]
        public bool ShowProgress { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress { get; set; }

        #endregion
    }
}