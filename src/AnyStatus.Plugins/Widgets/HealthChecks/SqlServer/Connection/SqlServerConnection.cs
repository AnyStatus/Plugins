using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Health Checks")]
    [DisplayName("SQL Server Connection")]
    public class SqlServerConnection : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "SQL Server Connection";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("Connection String")]
        [Description("")]
        public string ConnectionString { get; set; }
    }
}