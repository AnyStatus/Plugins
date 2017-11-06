using AnyStatus.API;
using System.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Microsoft.AspNetCore.WebUtilities;

namespace AnyStatus
{
    [CategoryOrder("TeamCity", 10)]
    [DisplayName("TeamCity Build")]
    [Description("TeamCity build status")]
    [DisplayColumn("Continuous Integration")]
    public class TeamCityBuild : Build, IMonitored, ICanOpenInBrowser, ICanTriggerBuild
    {
        private string url;

        [Url]
        [Required]
        [PropertyOrder(10)]
        [DisplayName("URL")]
        [Category("TeamCity")]
        [Description("TeamCity server URL address. For example: http://teamcity:8080")]
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                var uri = new Uri(value);
                url = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
                var queryString = QueryHelpers.ParseNullableQuery(uri.Query).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.FirstOrDefault());
                BuildTypeId = queryString.TryGetValue("buildTypeId", out string buildTypeId) ? buildTypeId : null;
                OnPropertyChanged(nameof(BuildTypeId));
                SourceControlBranch = queryString.Where(kvp => kvp.Key.StartsWith("branch_", StringComparison.Ordinal)).Select(kvp => kvp.Value).FirstOrDefault();
                OnPropertyChanged(nameof(SourceControlBranch));
            }
        }

        [Required]
        [PropertyOrder(20)]
        [Category("TeamCity")]
        [DisplayName("Build Type Id")]
        [Description("TeamCity build type id (Copy from TeamCity build URL address)")]
        public string BuildTypeId { get; set; }

        [PropertyOrder(25)]
        [Category("Source Control")]
        [DisplayName("Branch")]
        [Description("Optional. Branch to show status for")]
        public string SourceControlBranch { get; set; }

        [PropertyOrder(30)]
        [Category("TeamCity")]
        [DisplayName("Use Guest User")]
        [Description("Log in with TeamCity guest user. If checked, the User Name and Password are ignored")]
        public bool GuestUser { get; set; }

        [PropertyOrder(40)]
        [Category("TeamCity")]
        [DisplayName("User Name")]
        [Description("Optional.")]
        public string UserName { get; set; }

        [PropertyOrder(50)]
        [Category("TeamCity")]
        [PasswordPropertyText(true)]
        [Description("Optional.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }

        [PropertyOrder(60)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }

        public bool CanOpenInBrowser()
        {
            return State != State.None && State != State.Error;
        }

        public bool CanTriggerBuild()
        {
            return State != State.None && State != State.Error;
        }

        [XmlIgnore]
        [Browsable(false)]
        internal string StateText { get; set; }

        public override Notification CreateNotification()
        {
            if (State == State.Failed)
            {
                if (PreviousState == State.Error) return Notification.Empty; //bypass when network connection is restored.
                return new Notification($"{Name} failed. {StateText}", NotificationIcon.Error);
            }

            return base.CreateNotification();
        }
    }
}
