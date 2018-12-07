using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.DevOps.Microsoft.Azure.Widgets
{
    public class AzureDevOpsBuildWidget : AzureDevOpsWidget, IInitializable, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        public AzureDevOpsBuildWidget() : base(aggregate: false) { }

        [XmlIgnore]
        [Browsable(false)]
        public string BuildDefinitionId { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(40)]
        [DisplayName("Build Definition Name")]
        [Description("Required (case-sensitive). Enter your Visual Studio Team Services build definition name.")]
        public string BuildDefinitionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://{Connection.Account}.visualstudio.com/{Uri.EscapeDataString(Connection.Project)}/_build/index?definitionId={BuildDefinitionId}&_a=completed";

        public bool IsInitialized { get; set; }

        public override object Clone()
        {
            var clone = (AzureDevOpsBuildWidget)base.Clone();

            clone.BuildDefinitionId = null;

            return clone;
        }
    }
}
