using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceProcess;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("Windows Service")]
    [Description("Control and view the status of a local or remote windows service (requires administrative privileges).")]
    public class WindowsService : Widget,
        IHealthCheck, ISchedulable, IStartable, IStoppable, IRestartable
    {
        private const string Category = "Windows Service";

        public WindowsService()
        {
            Status = ServiceControllerStatus.Running;
        }

        [Required]
        [Category(Category)]
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [Category(Category)]
        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Category(Category)]
        public ServiceControllerStatus Status { get; set; }

        //todo: add Started & Stopped states and add logic to following methods:

        //public bool CanRestart()
        //{
        //    return State != State.None && State != State.Error;
        //}

        //public bool CanStart()
        //{
        //    return State != State.None && State != State.Error;
        //}

        //public bool CanStop()
        //{
        //    return State != State.None && State != State.Error;
        //}
    }
}