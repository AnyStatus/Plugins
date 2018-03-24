using AnyStatus.API;

namespace AnyStatus
{
    public class OpenVstsBuildPage : OpenWebPage<VSTSBuild_v1>
    {
        public OpenVstsBuildPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}