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
        private int progress;
        private bool showProgress = true;

        public Battery()
        {
            Name = "Battery";
        }

        [Category("Battery")]
        [DisplayName("Threshold")]
        [Description("Battery life percent threshold.")]
        public int BatteryLifePercentThreshold { get; set; }

        #region IReportProgress

        [Category("Battery")]
        [DisplayName("Show Progress")]
        public bool ShowProgress
        {
            get => showProgress;
            set
            {
                showProgress = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}