﻿using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Process Count")]
    [DisplayColumn("System Information")]
    [Description("Shows the number of running CPU processes")]
    public class ProcessCount : Sparkline, ISchedulable
    {
        public ProcessCount()
        {
            Name = "Process Count";
            Interval = 10;
            Units = IntervalUnits.Seconds;
        }

        [Category("Process Count")]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }
    }
}