namespace AnyStatus
{
    public class JenkinsCrumb
    {
        public string Crumb { get; set; }

        public string CrumbRequestField { get; set; }
    }

    public static class JenkinsCrumbExtensions
    {
        public static bool IsValid(this JenkinsCrumb crumb)
        {
            return crumb != null && !string.IsNullOrEmpty(crumb.Crumb) && !string.IsNullOrEmpty(crumb.CrumbRequestField);
        }
    }
}