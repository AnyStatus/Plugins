using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Code")]
    [DisplayName("PowerShell")]
    [Description("Run a PowerShell script and check the exit code.")]
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
        [Description("The PowerShell script file path.")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category(Category)]
        [Description("The script arguments.")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [Description("The script execution timeout in minutes.")]
        public int Timeout { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Exit Code")]
        [Description("Expected exit code. AnyStatus will change the status to \"Failed\" if the script returns a different exit code.")]
        public int ExitCode { get; set; }

        [PropertyOrder(50)]
        [Category(Category)]
        [DisplayName("Bypass Execution Policy")]
        [Description("Bypass PowerShell execution policy when running the script.")]
        public bool BypassExecutionPolicy { get; set; }
    }
}