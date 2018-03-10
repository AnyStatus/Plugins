using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("VSTS Build (Preview)")]
    [DisplayColumn("Continuous Integration")]
    [Description("Visual Studio Team Services - Build status and notifications")]
    public class VSTSBuild_v1 : VstsPlugin, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        private const string Category = "Build Definition";

        public VSTSBuild_v1() : base(aggregate: false) { }

        [Required]
        [Category(Category)]
        [PropertyOrder(40)]
        [DisplayName("Build Definition")]
        [Description("Required (case-sensitive). Enter your Visual Studio Team Services build definition name.")]
        public string DefinitionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public long? DefinitionId { get; set; }

        public string URL => $"https://{Account}.visualstudio.com/{Project}/_build/index?definitionId={DefinitionId}&_a=completed";

        public override object Clone()
        {
            var clone = (VSTSBuild_v1)base.Clone();

            clone.DefinitionId = null;

            return clone;
        }

        //public bool CanOpenInBrowser()
        //{
        //    return State != State.Error && DefinitionId != null;
        //}

        //public bool CanTriggerBuild()
        //{
        //    return State != State.Error && DefinitionId != null;
        //}
    }
}