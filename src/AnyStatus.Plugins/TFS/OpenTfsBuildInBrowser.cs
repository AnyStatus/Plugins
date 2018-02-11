using AnyStatus.API;

namespace AnyStatus
{
    public class OpenTfsBuildInBrowser : BaseTfsBuildHandler, IOpenInBrowser<TfsBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenTfsBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public override void Handle(TfsBuild item)
        {
            base.Handle(item);

            var uri = $"{item.Url}/{item.Collection}/{item.TeamProject}/_build?_a=completed&definitionId={item.BuildDefinitionId}";

            _processStarter.Start(uri.ToString());
        }
    }
}