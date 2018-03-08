using AnyStatus.API;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class SqlServerHealthCheck : ICheckHealth<SqlServerConnection>
    {
        public async Task Handle(HealthCheckRequest<SqlServerConnection> request, CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(request.DataContext.ConnectionString);

            try
            {
                await connection.OpenAsync().ConfigureAwait(false);

                request.DataContext.State = State.Ok;
            }
            catch (SqlException)
            {
                request.DataContext.State = State.Failed;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}