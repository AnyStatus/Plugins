using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("Socket Connection")]
    [Description("TCP or UDP socket connection health check.")]
    public class Port : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "Port";

        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(20)]
        public NetworkProtocol Protocol { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(30)]
        [DisplayName("Port Number")]
        [Range(0, ushort.MaxValue, ErrorMessage = "Port Number must be between 0 and 65535.")]
        public int PortNumber { get; set; }
    }
}