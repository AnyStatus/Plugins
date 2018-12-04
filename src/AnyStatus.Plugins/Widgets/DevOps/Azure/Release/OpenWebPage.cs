using AnyStatus.API;

namespace AnyStatus
{
    public class OpenVstsReleasePage : OpenWebPage<VSTSRelease_v1>
    {
        public OpenVstsReleasePage(IProcessStarter ps) : base(ps)
        {
        }
    }
}