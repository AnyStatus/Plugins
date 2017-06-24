using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    //http://www.m0interactive.com/archives/2008/02/06/how_to_calculate_network_bandwidth_speed_in_c_/
    //http://stackoverflow.com/questions/13600604/how-to-get-accurate-download-upload-speed-in-c-net

    [DisplayName("Download Speed")]
    [DisplayColumn("Network")]
    [Description("Monitor network download speed.")]
    public class DownloadSpeed : NetworkSpeed
    {
    }

    [DisplayName("Upload Speed")]
    [DisplayColumn("Network")]
    [Description("Monitor network upload speed.")]
    public class UploadSpeed : NetworkSpeed
    {
    }

    public abstract class NetworkSpeed : Metric, IMonitored
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

    public class NetworkInterfaceItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var items = new ItemCollection();

            var interfaces = from @interface in NetworkInterface.GetAllNetworkInterfaces()
                             select new Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item()
                             {
                                 DisplayName = @interface.Name,
                                 Value = @interface.Id
                             };

            items.AddRange(interfaces);

            return items;
        }
    }

    public class NetworkSpeedMonitor : IMonitor<DownloadSpeed>, IMonitor<UploadSpeed>
    {
        private enum Direction
        {
            Download,
            Upload,
        }

        public void Handle(DownloadSpeed item)
        {
            Handle(item, Direction.Download);
        }

        public void Handle(UploadSpeed item)
        {
            Handle(item, Direction.Upload);
        }

        [DebuggerStepThrough]
        private void Handle(NetworkSpeed item, Direction direction)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                throw new Exception("There are no available network connections.");

            var networkInterface = GetNetworkInterfaceById(item.NetworkInterfaceId);

            if (networkInterface == null)
                throw new Exception("The network interface was not found. Please select another network interface.");

            var startValue = networkInterface.GetIPv4Statistics();
            var starTime = DateTime.Now;

            Thread.Sleep(1000);

            var endValue = networkInterface.GetIPv4Statistics();
            var endTime = DateTime.Now;

            long totalBytes;

            totalBytes = direction == Direction.Upload ?
                endValue.BytesSent - startValue.BytesSent :
                endValue.BytesReceived - startValue.BytesReceived;

            var bitsPerSecond = (totalBytes * 8) / (endTime - starTime).TotalSeconds;

            item.Value = bitsPerSecond > 1000000 ?
                            Math.Round(bitsPerSecond / 1000000, 1) + " Mbps" :
                            Math.Round(bitsPerSecond / 1000, 1) + " Kbps";

            item.State = State.Ok;
        }

        private static NetworkInterface GetNetworkInterfaceById(string id)
        {
            return NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(k => k.Id.Equals(id));
        }
    }
}
