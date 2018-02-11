using AnyStatus.API;
using Microsoft.Web.Administration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("IIS Application Pool")]
    [Description("Monitor IIS application pool state")]
    public class IISApplicationPool : Widget, IMonitored
    {
        private const string Category = "Application Pool";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Host")]
        [Description("The IIS server host name or IP address. Note: you must be an administrator on the target machine.")]
        public string Host { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Application Pool Name")]
        [Description("")]
        public string ApplicationPoolName { get; set; }
    }

    public class ApplicationPoolMonitor : IMonitor<IISApplicationPool>
    {
        public void Handle(IISApplicationPool item)
        {
            using (var iis = ServerManager.OpenRemote(item.Host))
            {
                var appPool = iis.ApplicationPools[item.ApplicationPoolName];

                item.State = appPool.State == ObjectState.Started ? State.Ok : State.Failed;
            }
        }
    }
}