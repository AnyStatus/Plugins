using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus
{
    [DisplayName("Logical Drive Usage")]
    [DisplayColumn("System Information")]
    [Description("Shows the percentage of used space in a logical drive")]
    public class LogicalDriveUsage : Metric, ISchedulable, IReportProgress
    {
        public LogicalDriveUsage()
        {
            Name = "Logical Drive Usage";
            Symbol = "%";
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("Process CPU Usage")]
        [DisplayName("Drive")]
        [Description("The logical drive")]
        public string Drive { get; set; }

        [Required]
        [Category("Process CPU Usage")]
        [DisplayName("Percentage Type")]
        public PercentageType PercentageType { get; set; }

        [Category("Process CPU Usage")]
        [DisplayName("Show progress bar")]
        [Description("Should the status show a bar displaying how full the drive is?")]
        public bool ShowProgress { get; set; } = true;

        [XmlIgnore]
        [Browsable(false)]
        public int Progress
        {
            get => (Value is int) ? (int)Value : -1;
            set
            {
                Value = value;

                OnPropertyChanged();
            }
        }
    }
}