using AnyStatus.API;

namespace AnyStatus
{
    public class OpenWebPage : OpenWebPage<CoverallsCoveredPercent>
    {
        public OpenWebPage(IProcessStarter ps):base(ps)
        {
        }
    }
}