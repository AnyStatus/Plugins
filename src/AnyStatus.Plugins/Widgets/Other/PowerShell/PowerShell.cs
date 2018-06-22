using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("PowerShell")]
    [Description("PowerShell script health check.")]
    public class PowerShellScript : Widget, ISchedulable, IHealthCheck
    {
        private const string Category = "PowerShell";

        public PowerShellScript()
        {
            Timeout = 1;
            BypassExecutionPolicy = true;
        }

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("File Name")]
        [Description("The script file path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category(Category)]
        [Description("The script arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Exit Code")]
        [Description("Expected exit code.")]
        public int ExitCode { get; set; }

        [ReadOnly(true)]
        [PropertyOrder(50)]
        [Category(Category)]
        [DisplayName("Bypass Execution Policy")]
        [Description("Bypass PowerShell execution policy")]
        public bool BypassExecutionPolicy { get; set; }
    }
}