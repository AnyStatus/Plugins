using AnyStatus.API;
using AnyStatus.API.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class TeamCityBuildStatus : ICheckHealth<TeamCityBuild>
    {
        public async Task Handle(HealthCheckRequest<TeamCityBuild> request, CancellationToken cancellationToken)
        {
            var client = new TeamCityClient(new TeamCityConnection());

            request.DataContext.CopyTo(client.Connection);

            var build = await client.GetBuildDetailsAsync(request.DataContext).ConfigureAwait(false);

            request.DataContext.State = build.State;
            request.DataContext.Message = build.StatusText;
        }
    }
}