using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("VSTS Build")]
    [DisplayColumn("DevOps")]
    [Description("Visual Studio Team Services build status and notifications.")]
    public class VSTSBuild_v1 : AzureDevOpsWidget, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        public VSTSBuild_v1() : base(aggregate: false) { }

        [Required]
        [Category(Category)]
        [PropertyOrder(40)]
        [DisplayName("Build Definition Name")]
        [Description("Required (case-sensitive). Enter your Visual Studio Team Services build definition name.")]
        public string DefinitionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public long? DefinitionId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://{Account}.visualstudio.com/{Uri.EscapeDataString(Project)}/_build/index?definitionId={DefinitionId}&_a=completed";

        public override object Clone()
        {
            var clone = (VSTSBuild_v1)base.Clone();

            clone.DefinitionId = null;

            return clone;
        }
    }
}