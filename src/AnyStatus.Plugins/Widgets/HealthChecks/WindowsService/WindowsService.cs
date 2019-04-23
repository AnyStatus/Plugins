using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceProcess;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("Windows Service")]
    [Description("Control and view the status of a local or remote windows service. Requires administrative privileges.")]
    public class WindowsService : Widget, IHealthCheck, ISchedulable, IStartable, IStoppable, IRestartable
    {
        private const string Category = "Windows Service";

        public WindowsService()
        {
            Status = ServiceControllerStatus.Running;
        }

        [Required]
        [Category(Category)]
        [DisplayName("Service Name")]
        [Description("Required. The windows service name.")]
        [Editor(typeof(WindowsServiceNameEditor), typeof(WindowsServiceNameEditor))]
        public string ServiceName { get; set; }

        [Category(Category)]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Category(Category)]
        [Description("Required. The expected windows service status.")]
        public ServiceControllerStatus Status { get; set; }
    }
}