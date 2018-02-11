namespace AnyStatus
{
    public interface IJenkinsPlugin
    {
        string URL { get; set; }

        string UserName { get; set; }

        string ApiToken { get; set; }

        bool IgnoreSslErrors { get; set; }

        bool CSRF { get; set; }
    }
}