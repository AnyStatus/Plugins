using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("SQL Server Connection")]
    [Description("Monitor SQL Server database connectivity.")]
    public class SqlServerConnection : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "SQL Server Connection";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Connection String")]
        [Description("Required. The database connection string.")]
        public string ConnectionString { get; set; }
    }
}