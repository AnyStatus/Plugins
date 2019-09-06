using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayName("Battery")]
    [DisplayColumn("System Information")]
    [XmlType(TypeName = "Battery_v1")]
    [Description("Display the battery status information.")]
    public class Battery : Metric, ISchedulable, IReportProgress
    {
        private int _progress;
        private bool _showProgress;

        public Battery()
        {
            Name = "Battery";
            ShowProgress = true;
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
            get => _showProgress;
            set
            {
                _showProgress = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}