using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

//todo:
// Use TeamCity build URL instead of Server URL + Build Type Id.

namespace AnyStatus
{
    [CategoryOrder("TeamCity", 10)]
    [DisplayName("Build")]
    [Description("TeamCity build status")]
    [DisplayColumn("TeamCity")]
    public class TeamCityBuild : Build, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        private const string Category = "TeamCity";

        private string _url;
        private bool _guestUser;

        /// <summary>
        /// TeamCity Server URL.
        /// </summary>
        [Url]
        [Required]
        [PropertyOrder(10)]
        [DisplayName("TeamCity URL")]
        [Category(Category)]
        [Description("TeamCity server URL address. For example: http://teamcity:8080")]
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;

                if (_url.EndsWith("/"))
                {
                    _url = _url.Remove(_url.Length - 1);
                }
            }
        }

        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Ignore SSL errors")]
        public bool IgnoreSslErrors { get; set; }

        /// <summary>
        /// Web Page URL.
        /// </summary>
        [Browsable(false)]
        public string URL => $"{Url}/viewType.html?buildTypeId={BuildTypeId}";

        [Required]
        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("Build Type Id")]
        [Description("TeamCity build type id. Copy from TeamCity build URL address.")]
        public string BuildTypeId { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Branch")]
        [Description("Optional. Branch to show status for.")]
        public string SourceControlBranch { get; set; }

        [PropertyOrder(50)]
        [Category(Category)]
        [DisplayName("Use guest user")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Log in with TeamCity guest user. If checked, the User Name and Password are ignored")]
        public bool GuestUser
        {
            get => _guestUser;
            set
            {
                _guestUser = value;

                SetPropertyVisibility(nameof(Token), !_guestUser);
            }
        }

        [Browsable(true)]
        [PropertyOrder(60)]
        [Category(Category)]
        [PasswordPropertyText(true)]
        [Description("Optional.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Token { get; set; }

        public override Notification CreateNotification()
        {
            if (State == State.Failed)
            {
                return new Notification($"{Name} failed. {Message}", NotificationIcon.Error);
            }

            return base.CreateNotification();
        }
    }
}