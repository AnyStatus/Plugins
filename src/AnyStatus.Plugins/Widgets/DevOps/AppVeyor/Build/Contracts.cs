namespace AnyStatus
{
    public class AppVeyorBuildResponse
    {
        public AppVeyorBuildDetails Build { get; set; }
    }

    public class AppVeyorBuildDetails
    {
        public string Version { get; set; }

        public string Status { get; set; }
    }
}
