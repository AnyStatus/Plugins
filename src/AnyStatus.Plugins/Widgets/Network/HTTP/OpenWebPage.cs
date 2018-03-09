using AnyStatus.API;

namespace AnyStatus
{
    public class OpenHttpWebPage : OpenWebPage<HttpStatus>
    {
        public OpenHttpWebPage(IProcessStarter ps) : base(ps) { }
    }
}
