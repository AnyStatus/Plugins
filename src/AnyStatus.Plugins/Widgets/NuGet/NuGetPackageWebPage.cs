using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.NuGet
{
    public class NuGetPackageWebPage : OpenWebPage<NuGetPackageWidget>
    {
        public NuGetPackageWebPage(IProcessStarter ps) : base(ps) { }
    }
}
