using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    public abstract class NetworkSpeed : Metric, IHealthCheck, ISchedulable
    {
        public NetworkSpeed()
        {
            Interval = 1;
        }

        [Required]
        [DisplayName("Network Interface")]
        [ItemsSource(typeof(NetworkInterfaceItemsSource))]
        public string NetworkInterfaceId { get; set; }
    }
}