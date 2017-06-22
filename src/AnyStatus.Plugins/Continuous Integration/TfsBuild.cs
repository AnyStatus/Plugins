using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("TFS/VSTS Build")]
    [DisplayColumn("Continuous Integration")]
    [Description("Microsoft Team Foundation Server or Visual Studio Team Services build status")]
    public class TfsBuild : Build, ISchedulable, ICanOpenInBrowser, ICanTriggerBuild
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

    public abstract class BaseTfsBuildHandler
    {
        [DebuggerStepThrough]
        public virtual void Handle(TfsBuild item)
        {
            if (item.BuildDefinitionId <= 0)
            {
                item.BuildDefinitionId = GetBuildDefinitionIdAsync(item).Result;
            }
        }

        public virtual async Task HandleAsync(TfsBuild item)
        {
            if (item.BuildDefinitionId <= 0)
            {
                item.BuildDefinitionId = await GetBuildDefinitionIdAsync(item);
            }
        }

        protected async Task<int> GetBuildDefinitionIdAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/definitions?api-version=2.0&name={item.BuildDefinition}";

                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDefinitionResponse = new JavaScriptSerializer().Deserialize<BuildDefinitionResponse>(content);

                    return buildDefinitionResponse.Value.First().Id;
                }
            }
        }
    }

    public class TfsBuildMonitor : BaseTfsBuildHandler, IMonitor<TfsBuild>
    {
        [DebuggerStepThrough]
        public override void Handle(TfsBuild item)
        {
            base.Handle(item);

            var buildDetails = GetBuildDetailsAsync(item).Result;

            if (buildDetails.Status == "notStarted" || buildDetails.Status == "inProgress")
            {
                item.State = State.Running;
                return;
            }

            switch (buildDetails.Result)
            {
                case "notStarted":
                    item.State = State.Running;
                    break;
                case "succeeded":
                    item.State = State.Ok;
                    break;
                case "failed":
                    item.State = State.Failed;
                    break;
                case "partiallySucceeded":
                    item.State = State.PartiallySucceeded;
                    break;
                case "canceled":
                    item.State = State.Canceled;
                    break;
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?definitions={item.BuildDefinitionId}&$top=1&api-version=2.0";

                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetailsResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetailsResponse>(content);

                    return buildDetailsResponse.Value.First();
                }
            }
        }
    }

    public class TriggerTfsBuild : BaseTfsBuildHandler, ITriggerBuild<TfsBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerTfsBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        [DebuggerStepThrough]
        public override async Task HandleAsync(TfsBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await base.HandleAsync(build);

            await QueueNewBuild(build);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?api-version=2.0";

                    var request = new QueueNewBuildRequest
                    {
                        Definition = new Definition
                        {
                            Id = item.BuildDefinitionId
                        }
                    };

                    var json = new JavaScriptSerializer().Serialize(request);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }

    public class OpenTfsBuildInBrowser : BaseTfsBuildHandler, IOpenInBrowser<TfsBuild>
    {
        private readonly ILogger _logger;
        private readonly IProcessStarter _processStarter;

        public OpenTfsBuildInBrowser(IProcessStarter processStarter, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public override void Handle(TfsBuild item)
        {
            base.Handle(item);

            var uri = $"{item.Url}/{item.Collection}/{item.TeamProject}/_build?_a=completed&definitionId={item.BuildDefinitionId}";

            _processStarter.Start(uri.ToString());
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

    #endregion
}
