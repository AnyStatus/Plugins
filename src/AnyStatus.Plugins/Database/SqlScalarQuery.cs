using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Sql Scalar Query")]
    [DisplayColumn("Database")]
    public class SqlScalarQuery : Metric, IMonitored
    {
        private const string Category = "SQL Scalar Query";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Connection String")]
        [Description("")]
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
            //string sql =
            //    "INSERT INTO Production.ProductCategory (Name) VALUES (@Name); "
            //    + "SELECT CAST(scope_identity() AS int)";

            using (var connection = new SqlConnection(item.ConnectionString))
            {
                var cmd = new SqlCommand(item.SqlQuery, connection);
                //cmd.Parameters.Add("@Name", SqlDbType.VarChar);
                //cmd.Parameters["@name"].Value = newName;

                try
                {
                    connection.Open();

                    item.Value = (string)cmd.ExecuteScalar();

                    item.State = State.Ok;
                }
                catch (SqlException sex)
                {
                    _logger.Error(sex);

                    item.State = State.Failed;
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
