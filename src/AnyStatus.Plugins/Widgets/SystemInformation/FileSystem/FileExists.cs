using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AnyStatus
{
    [DisplayName("File Exists")]
    [DisplayColumn("System Information")]
    [Description("Determines whether the specified file exists.")]
    public class FileExists : Widget, IHealthCheck, ISchedulable
    {
        public FileExists()
        {
            Name = "File Exists";
        }

        [Required]
        [Category("File Exists")]
        [Description("Required. The file path to check.")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string Path { get; set; }
    }

    public class FileExistsCheck : RequestHandler<HealthCheckRequest<FileExists>>
    {
        protected override void HandleCore(HealthCheckRequest<FileExists> request)
        {
            request.DataContext.State = File.Exists(request.DataContext.Path) ? State.Ok : State.Failed;
        }
    }
}
