using System;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("TFS 15 Work Item")]
    public class TfsWorkItem : Item, IScheduledItem
    {
    }

    public class TfsWorkItemMonitor : IMonitor<TfsWorkItem>
    {
        public void Handle(TfsWorkItem item)
        {
            throw new NotImplementedException();
        }
    }
}
