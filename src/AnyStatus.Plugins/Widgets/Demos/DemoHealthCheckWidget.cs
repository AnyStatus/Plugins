using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins
{
    /// <summary>
    /// Demo health-check Widget.
    /// </summary>
    [Browsable(false)]
    public class DemoHealthCheckWidget : HealthCheck, ISchedulable, IWebPage
    {
        [Required]
        [Category("My Widget")] //default is "General".
        [DisplayName("My property name")]
        [Description("My property description.")]
        [PropertyOrder(1)] //sets the order in which the property will appear in the editor. Smaller is higher.
        public string MyProperty { get; set; }

        /// <summary>
        /// Use the XmlIgnore attribute to indicate that the property should not be persisted between sessions.
        /// </summary>
        [XmlIgnore]
        [PropertyOrder(2)]
        public int NonPersistedProperty { get; set; }

        /// <summary>
        /// Use the Browsable attribute to hide properties.
        /// </summary>
        [Browsable(false)]
        public int MyHiddenProperty { get; set; }

        /// <summary>
        /// "Open in Browser" URL address.
        /// </summary>
        public string URL => "https://www.anystat.us";
    }
}
