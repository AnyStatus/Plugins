using AnyStatus.API;
using System;
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
    [DisplayColumn("DevOps")]
    [Description("Microsoft Team Foundation Server or Visual Studio Team Services build status")]
    public class TfsBuild : Build, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        private const string Category = "Build Definition";

        public TfsBuild()
        {
            Collection = "DefaultCollection";
        }

        /// <summary>
        /// Server URL.
        /// </summary>
        [Url]
        [Required]
        [Category(Category)]
        [PropertyOrder(10)]
        [Description("Required. Visual Studio Team Services account (https://{account}.visualstudio.com) or TFS server (http://{server:port}/tfs) URL address.")]
        public string Url { get; set; }

        /// <summary>
        /// Build Web Page URL.
        /// </summary>
        [Browsable(false)]
        public string URL => $"{Url}/{Uri.EscapeDataString(Collection)}/{Uri.EscapeDataString(TeamProject)}/_build?_a=completed&definitionId={BuildDefinitionId}";

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