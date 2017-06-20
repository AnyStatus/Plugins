using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Custom")]
    [DisplayName("PowerShell")]
    [Description("Monitor the exit code of a PowerShell script")]
    public class PowerShellScript : Item, IScheduledItem
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

    public class PowerShellScriptMonitor : IMonitor<PowerShellScript>
    {
        private readonly IProcessStarter _processStarter;

        public PowerShellScriptMonitor(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        [DebuggerStepThrough]
        public void Handle(PowerShellScript item)
        {
            Validate(item);

            var executionPolicy = item.BypassExecutionPolicy ? "ByPass" : "Restricted";

            var info = new ProcessStartInfo
            {
                FileName = "PowerShell.exe",
                Arguments = $"-ExecutionPolicy {executionPolicy} -File \"{item.FileName}\" {item.Arguments}",
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var exitCode = _processStarter.Start(info, TimeSpan.FromMinutes(item.Timeout));

            item.State = exitCode == 0 ? State.Ok : State.Failed;
        }

        [DebuggerStepThrough]
        private static void Validate(PowerShellScript item)
        {
            if (!File.Exists(item.FileName))
                throw new FileNotFoundException(item.FileName);
        }
    }
}
