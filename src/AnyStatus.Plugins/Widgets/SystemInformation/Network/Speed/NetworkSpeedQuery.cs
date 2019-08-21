using AnyStatus.API;
using System;
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
            Upload
        }

        public Task Handle(MetricQueryRequest<DownloadSpeed> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext, Direction.Download);
        }

        public Task Handle(MetricQueryRequest<UploadSpeed> request, CancellationToken cancellationToken)
        {
            return Handle(request.DataContext, Direction.Download);
        }

        private static async Task Handle(NetworkSpeed widget, Direction direction)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new Exception("There are no available network interfaces.");

            var networkInterface = GetNetworkInterfaceById(widget.NetworkInterfaceId);
            var startValue = networkInterface.GetIPv4Statistics();
            var starTime = DateTime.Now;

            await Task.Delay(1000).ConfigureAwait(false);

            var endValue = networkInterface.GetIPv4Statistics();
            var endTime = DateTime.Now;

            var totalBytes = direction == Direction.Upload ?
                endValue.BytesSent - startValue.BytesSent :
                endValue.BytesReceived - startValue.BytesReceived;

            widget.Value = totalBytes / (endTime - starTime).TotalSeconds;
            
            widget.State = State.Ok;
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