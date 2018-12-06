namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Contracts
{

    public class AzureDevOpsReleaseDetails : AzureDevOpsRelease
    {
        public AzureDevOpsReleaseEnvironment[] Environments { get; set; }
    }
}