using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("IIS Application Pool")]
    [Description("Monitor the status of a local or remote IIS application pool. Enables starting, stopping and restarting the application pool. Note: your user must be an administrator on the remote server.")]
    public class IISApplicationPool : Widget, IHealthCheck, ISchedulable, IStartable, IStoppable, IRestartable
    {
        private const string Category = "IIS Application Pool";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Server Name")]
        [Description("Required. The name of the server to connect to. Use \"localhost\" for your local computer.")]
        public string Host { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Name")]
        [Description("Required. The name of the application pool.")]
        public string ApplicationPoolName { get; set; }
    }
}