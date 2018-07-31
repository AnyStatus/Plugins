using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    public abstract class VstsPlugin : Build
    {
        protected const string Category = "VSTS";

        protected VstsPlugin(bool aggregate) : base(aggregate)
        {
        }

        [Url]
        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Required. Enter your Visual Studio Team Services account name.")]
        public string Account { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(20)]
        [DisplayName("Team Project Name")]
        [Description("Required (case-sensitive). Enter your Visual Studio Team Services project name.")]
        public string Project { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("Optional. Enter the user name of your Visual Studio Team Services account. " +
                     "To authenticate with a Personal Access Token, leave the User Name empty.")]
        public string UserName { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Password (Token)")]
        [Description("Optional. Enter the password or Personal Access Token of your Visual Studio Team Services account. " +
                     "To authenticate with a Personal Access Token, leave the User Name empty.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }
    }
}