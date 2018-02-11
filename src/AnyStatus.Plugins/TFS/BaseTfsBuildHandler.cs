using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    public abstract class BaseTfsBuildHandler
    {
        [DebuggerStepThrough]
        public virtual void Handle(TfsBuild buildDefinition)
        {
            HandleAsync(buildDefinition).Wait();
        }

        public virtual async Task HandleAsync(TfsBuild buildDefinition)
        {
            if (buildDefinition.BuildDefinitionId <= 0)
                buildDefinition.BuildDefinitionId = await GetBuildDefinitionIdAsync(buildDefinition).ConfigureAwait(false);
        }

        protected async Task<int> GetBuildDefinitionIdAsync(TfsBuild buidDefinition)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(buidDefinition.UserName) || string.IsNullOrEmpty(buidDefinition.Password); ;

                var httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (handler.UseDefaultCredentials == false)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{buidDefinition.UserName}:{buidDefinition.Password}")));
                }

                var url = $"{buidDefinition.Url}/{buidDefinition.Collection}/{buidDefinition.TeamProject}/_apis/build/definitions?api-version=2.0&name={buidDefinition.BuildDefinition}";

                var response = await httpClient.GetAsync(url).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var buildDefinitionResponse = new JavaScriptSerializer().Deserialize<BuildDefinitionResponse>(content);

                if (buildDefinitionResponse == null || buildDefinitionResponse.Value == null || !buildDefinitionResponse.Value.Any())
                    throw new Exception("Build definition id was not found.");

                return buildDefinitionResponse.Value.First().Id;
            }
        }
    }
}