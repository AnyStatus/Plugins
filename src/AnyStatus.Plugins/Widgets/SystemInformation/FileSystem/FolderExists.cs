using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AnyStatus
{
    [DisplayName("Folder Exists")]
    [DisplayColumn("System Information")]
    [Description("Determines whether the specified folder exists.")]
    public class FolderExists : Widget, IHealthCheck, ISchedulable
    {
        public FolderExists()
        {
            Name = "Folder Exists";
        }

        [Required]
        [Category("Folder Exists")]
        [Description("Required. The folder path to check.")]
        [Editor(typeof(FolderEditor), typeof(FolderEditor))]
        public string Path { get; set; }
    }

    public class FolderExistsCheck : RequestHandler<HealthCheckRequest<FolderExists>>
    {
        protected override void HandleCore(HealthCheckRequest<FolderExists> request)
        {
            request.DataContext.State = Directory.Exists(request.DataContext.Path) ? State.Ok : State.Failed;
        }
    }
}
