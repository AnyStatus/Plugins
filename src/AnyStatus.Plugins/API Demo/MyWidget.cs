using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.API_Demo
{
    /// <summary>
    /// Widget properties and attributes example.
    /// </summary>
    [Browsable(false)]
    public class MyWidget : Widget, ISchedulable, IWebPage
    {
        [Required]
        [Category("My Widget")] //default is "General".
        [DisplayName("My property name")]
        [Description("My property description.")]
        [PropertyOrder(1)] //sets the order in which the property will appear in the category. Smaller is higher.
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
        string IWebPage.URL
        {
            get
            {
                return "https://anystat.us";
            }
        }
    }
}
