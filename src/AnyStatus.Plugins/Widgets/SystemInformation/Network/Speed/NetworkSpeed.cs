using AnyStatus.API;
using AnyStatus.API.Common.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    public abstract class NetworkSpeed : Sparkline, ISchedulable
    {
        protected NetworkSpeed()
        {
            Interval = 1;
        }

        [Required]
        [DisplayName("Network Interface")]
        [ItemsSource(typeof(NetworkInterfaceItemsSource))]
        public string NetworkInterfaceId { get; set; }

        public override string ToString()
        {
            return BytesFormatter.Format(Convert.ToInt64(Value));
        }
    }
}