using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("Travis CI Build")]
    [DisplayColumn("Continuous Integration")]
    public class TravisCIBuild : Build, IMonitored
    {
    }

    public class TravisCIBuildMonitor : IMonitor<TravisCIBuild>
    {
        public void Handle(TravisCIBuild item)
        {
            throw new NotImplementedException();
        }
    }
}
