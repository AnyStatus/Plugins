using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

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
        [Editor(typeof(MultilineTextBoxEditor), typeof(ITypeEditor))]
        public string SqlQuery { get; set; }
    }

    public class SqlScalarQueryMonitor : IMonitor<SqlScalarQuery>
    {
        //todo: make async

        private readonly ILogger _logger;

        public SqlScalarQueryMonitor(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public void Handle(SqlScalarQuery scalarQuery)
        {
            using (var connection = new SqlConnection(scalarQuery.ConnectionString))
            {
                var cmd = new SqlCommand(scalarQuery.SqlQuery, connection);

                try
                {
                    connection.Open();

                    scalarQuery.Value = (Int32)cmd.ExecuteScalar();

                    scalarQuery.State = State.Ok;
                }
                catch (Exception ex)
                {
                    scalarQuery.State = State.Error;

                    _logger.Error(ex);
                }
            }
        }
    }
}