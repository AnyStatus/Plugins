using AnyStatus.API;
using System;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("TFS 15 Work Item")]
    public class TfsWorkItem : API.Item, ISchedulable
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
