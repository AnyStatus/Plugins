using AnyStatus.API;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class SqlScalarQueryHandler : IMetricQuery<SqlScalarQuery>
    {
        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<SqlScalarQuery> request, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(request.DataContext.ConnectionString))
            {
                var command = new SqlCommand(request.DataContext.SqlQuery, connection);

                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                request.DataContext.Value = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                request.DataContext.State = State.Ok;
            }
        }
    }
}