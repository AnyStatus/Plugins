using AnyStatus.API;

namespace AnyStatus
{
    public class OpenTfsBuildWebPage : OpenWebPage<TfsBuild>
    {
        public OpenTfsBuildWebPage(IProcessStarter ps) : base(ps)
        {
        }
    }
}