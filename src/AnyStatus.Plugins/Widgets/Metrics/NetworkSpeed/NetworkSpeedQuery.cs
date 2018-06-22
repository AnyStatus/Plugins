using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class NetworkSpeedQuery : IMetricQuery<DownloadSpeed>, IMetricQuery<UploadSpeed>
    {
        private enum Direction
        {
            Download,
            Upload,
        }

        public Task Handle(MetricQueryRequest<DownloadSpeed> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext, Direction.Download);
        }

        public Task Handle(MetricQueryRequest<UploadSpeed> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext, Direction.Download);
        }

        [DebuggerStepThrough]
        private async Task Handle(NetworkSpeed item, Direction direction)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                throw new Exception("There are no available network connections.");

            var networkInterface = GetNetworkInterfaceById(item.NetworkInterfaceId);
            var startValue = networkInterface.GetIPv4Statistics();
            var starTime = DateTime.Now;

            await Task.Delay(1000).ConfigureAwait(false);

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
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(k => k.Id.Equals(id));

            if (networkInterface == null)
                throw new Exception("The network interface was not found.");

            return networkInterface;
        }
    }
}