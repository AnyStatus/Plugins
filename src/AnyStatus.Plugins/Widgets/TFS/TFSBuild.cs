using AnyStatus.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    /// <summary>
    /// This plugin has been replaced by the new VSTS plugin.
    /// </summary>
    [DisplayName("TFS Build")]
    [DisplayColumn("Continuous Integration")]
    [Description("Microsoft Team Foundation Server or Visual Studio Team Services build status")]
    public class TfsBuild : Build, IMonitored, ICanOpenInBrowser, ICanTriggerBuild
    {
        private const string Category = "Build Definition";

        public TfsBuild()
        {
            Collection = "DefaultCollection";
        }

        [Url]
        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Required. Visual Studio Team Services account (https://{account}.visualstudio.com) or TFS server (http://{server:port}/tfs)")]
        public string Url { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(20)]
        [Description("Required. The collection name. Default: DefaultCollection")]
        public string Collection { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(30)]
        [DisplayName("Team Project")]
        [Description("Required. The team project name.")]
        public string TeamProject { get; set; }

        [Required]
        [Category(Category)]
        [PropertyOrder(40)]
        [DisplayName("Build Definition")]
        [Description("Required. The build definition name.")]
        public string BuildDefinition { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int BuildDefinitionId { get; set; }

        [PropertyOrder(50)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("Optional.")]
        public string UserName { get; set; }

        [Category(Category)]
        [PropertyOrder(60)]
        [DisplayName("Password (Token)")]
        [Description("Optional.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }

        public bool CanOpenInBrowser()
        {
            return State != State.Error && BuildDefinitionId != 0;
        }

        public bool CanTriggerBuild()
        {
            return State != State.Error && BuildDefinitionId != 0;
        }

        public override object Clone()
        {
            var clone = base.Clone() as TfsBuild;

            clone.BuildDefinitionId = 0;

            return clone;
        }
    }

    #region Contracts

    internal class QueueNewBuildRequest
    {
        public Definition Definition { get; set; }
    }

    internal class Definition
    {
        public int Id { get; set; }
    }

    internal class BuildDefinitionResponse
    {
        public List<BuildDefinitionDetails> Value { get; set; }
    }

    internal class BuildDefinitionDetails
    {
        public int Id { get; set; }
    }

    internal class TfsBuildDetailsResponse
    {
        public List<TfsBuildDetails> Value { get; set; }
    }

    internal class TfsBuildDetails
    {
        public string Result { get; set; }

        public string Status { get; set; }
    }

    #endregion Contracts
}