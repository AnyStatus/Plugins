using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    [DisplayColumn("Database")]
    [DisplayName("SQL Scalar Query")]
    [Description("Executes the query, and shows the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.")]
    public class SqlScalarQuery : Metric, ISchedulable
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
}