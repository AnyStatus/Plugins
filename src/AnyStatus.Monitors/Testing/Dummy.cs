using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("Dummy")]
    [Description("Dummy monitor for tests")]
    public class Dummy : Item, IScheduledItem
    {
        public int Counter { get; set; }
    }

    public class DummyMonitor : IMonitor<Dummy>
    {
        public void Handle(Dummy item)
        {
            item.Counter += 1;
            item.State = State.Ok;
        }
    }
}
