using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("IIS Application Pool")]
    [Description("Control and view the status of a local or remote IIS application pool (requires administrative privileges).")]
    public class IISApplicationPool : Widget, IHealthCheck, ISchedulable, IStartable, IStoppable, IRestartable
    {
        private const string Category = "Application Pool";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Host")]
        [Description("Required. IIS server host name or IP address. Requires administrative privileges on the target machine.")]
        public string Host { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Name")]
        [Description("Required.")]
        public string ApplicationPoolName { get; set; }
    }
}