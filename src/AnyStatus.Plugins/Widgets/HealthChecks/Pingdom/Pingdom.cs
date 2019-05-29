using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Pingdom")]
    [DisplayColumn("Health Checks")]
    [Description("Single or multiple Pingdom health checks.")]
    public class Pingdom : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "Pingdom";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("API Key")]
        [Description("Required. Pingdom API key. Generate your API Key using Pingdom Control Panel.")]
        public string ApiKey { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("Required. Pingdom account user name (Email).")]
        public string UserName { get; set; }

        [Required]
        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("Password")]
        [Description("Required. Pingdom account password.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Check Id")]
        [Description("Optional. Leave empty for the overall status of all checks. The Check Id can be found in the URL address of the check.")]
        public string CheckId { get; set; }
    }
}