namespace AnyStatus
{
    /// <summary>
    /// TeamCity server connection.
    /// Note: this class is mapped to TeamCityBuild class.
    /// </summary>
    public class TeamCityConnection
    {
        /// <summary>
        /// TeamCity server URL address.
        /// </summary>
        public string Url { get; set; }

        public bool GuestUser { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IgnoreSslErrors { get; set; }
    }
}