using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("SQL Server Connection")]
    [DisplayColumn("Database")]
    public class SqlServerConnection : Plugin, IAmMonitored
    {
        private const string Category = "SQL Server Connection";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Connection String")]
        [Description("")]
        public string ConnectionString { get; set; }
    }

    public class SqlServerConnectionMonitor : IMonitor<SqlServerConnection>
    {
        public void Handle(SqlServerConnection item)
        {
            var connection = new SqlConnection(item.ConnectionString);

            try
            {
                connection.Open();

                item.State = State.Ok;
            }
            catch (SqlException)
            {
                item.State = State.Failed;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
