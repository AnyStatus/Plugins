using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Database")]
    [DisplayName("SQL Scalar Query")]
    [Description("Executes the query, and shows the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.")]
    public class SqlScalarQuery : Metric, IMonitored
    {
        private const string Category = "SQL Scalar Query";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Connection String")]
        [Description("Database connection string.")]
        public string ConnectionString { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("SQL Query")]
        [Description("")]
        public string SqlQuery { get; set; }
    }

    public class SqlScalarQueryMonitor : IMonitor<SqlScalarQuery>
    {
        private readonly ILogger _logger;

        public SqlScalarQueryMonitor(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public void Handle(SqlScalarQuery item)
        { 
            using (var connection = new SqlConnection(item.ConnectionString))
            {
                var cmd = new SqlCommand(item.SqlQuery, connection);

                try
                {
                    connection.Open();

                    item.Value = (Int32)cmd.ExecuteScalar();

                    item.State = State.Ok;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);

                    item.State = State.Error;
                }
            }
        }
    }
}
