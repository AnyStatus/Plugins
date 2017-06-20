using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace AnyStatus
{
    [DisplayName("Ping")]
    [DisplayColumn("Network")]
    [Description("Test the reachability of a host")]
    public class Ping : Item, IScheduledItem
    {
        [Required]
        [Category("Ping")]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }
    }

    public class PingMonitor : IMonitor<Ping>
    {
        [DebuggerStepThrough]
        public void Handle(Ping item)
        {
            using (var ping = new System.Net.NetworkInformation.Ping())
            {
                var reply = ping.Send(item.Host);

                item.State = (reply.Status == IPStatus.Success) ? State.Ok : State.Failed;
            }
        }
    }
}
