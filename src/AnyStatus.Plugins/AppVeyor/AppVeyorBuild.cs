using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("AppVeyor Build")]
    [Description("AppVeyor build status")]
    [DisplayColumn("Continuous Integration")]
    public class AppVeyorBuild : Build, IMonitored, ICanOpenInBrowser, ICanTriggerBuild
    {
        [Required]
        [Category("AppVeyor")]
        [DisplayName("Account Name")]
        [Description("Required. AppVeyor account name.")]
        public string AccountName { get; set; }

        [Required]
        [Category("AppVeyor")]
        [DisplayName("Project Slug")]
        [Description("Required. Project slug is the last part of the AppVeyor project URL. For example: https://ci.appveyor.com/project/AccountName/Project-Slug")]
        public string ProjectSlug { get; set; }

        [Required]
        [Category("AppVeyor")]
        [DisplayName("API Token")]
        [Description("Required. AppVeyor API token.")]
        public string ApiToken { get; set; }

        public bool CanOpenInBrowser()
        {
            return State != State.None && State != State.Error;
        }

        public bool CanTriggerBuild()
        {
            return State != State.None && State != State.Error;
        }
    }
}
