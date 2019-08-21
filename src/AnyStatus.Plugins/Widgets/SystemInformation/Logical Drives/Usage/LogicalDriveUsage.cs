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
        private int _progress;

        public LogicalDriveUsage()
        {
            Name = "Logical Drive Usage";
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Required]
        [Category("Logical Drive Usage")]
        [DisplayName("Drive")]
        [Description("The logical drive")]
        [Editor(typeof(LogicalDriveNameEditor), typeof(LogicalDriveNameEditor))]
        public string Drive { get; set; }

        [Required]
        [Category("Logical Drive Usage")]
        [DisplayName("Percentage Type")]
        public PercentageType PercentageType { get; set; }

        [Category("Logical Drive Usage")]
        [DisplayName("Show progress bar")]
        [Description("Should the status show a bar displaying how full the drive is?")]
        public bool ShowProgress { get; set; } = true;

        [Category("Logical Drive Usage")]
        [DisplayName("Error percentage")]
        [Description("At what percentage should this drive error?")]
        public int ErrorPercentage { get; set; }

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

        public override string ToString()
        {
            return $"{Value}%";
        }
    }
}