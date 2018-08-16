using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayColumn("Code")]
    [DisplayName("Batch Script")]
    [Description("Run a batch script and check the exit code.")]
    public class BatchFile : Widget, IHealthCheck, ISchedulable
    {
        private const string Category = "Batch";

        public BatchFile()
        {
            Timeout = 1;
        }

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("File Name")]
        [Description("The batch script to run.")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category(Category)]
        [Description("The arguments to use when running the batch script.")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [Description("The batch script execution timeout in minutes.")]
        public int Timeout { get; set; }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Exit Code")]
        [Description("Expected exit code. AnyStatus will change the status to \"Failed\" if the script returns a different exit code.")]
        public int ExitCode { get; set; }
    }
}