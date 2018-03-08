using AnyStatus.API;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class SqlScalarQueryHandler : IMetricQuery<SqlScalarQuery>
    {
        private readonly ILogger _logger;

        public SqlScalarQueryHandler(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        [DebuggerStepThrough]
        public async Task Handle(MetricQueryRequest<SqlScalarQuery> request, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(request.DataContext.ConnectionString))
            {
                var cmd = new SqlCommand(request.DataContext.SqlQuery, connection);

                try
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    request.DataContext.Value = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                    request.DataContext.State = State.Ok;
                }
                catch (Exception ex)
                {
                    request.DataContext.State = State.Error;

                    _logger.Error(ex);
                }
            }
        }
    }
}