using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("TCP/UDP Check")]
    [Description("Monitor TCP/UDP connectivity for any host and port.")]
    public class Port : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "TCP/UDP Check";

        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Required. The host name or IP address.")]
        public string Host { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(20)]
        [Description("Required. The network protocol.")]
        public NetworkProtocol Protocol { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(30)]
        [DisplayName("Port Number")]
        [Description("Required. A port number between 0 and 65535.")]
        [Range(0, ushort.MaxValue, ErrorMessage = "Port number must be a number between 0 and 65535.")]
        public int PortNumber { get; set; }
    }
}