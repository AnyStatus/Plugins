using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Upload Speed")]
    [DisplayColumn("System Information")]
    [Description("Monitor network upload speed.")]
    public class UploadSpeed : NetworkSpeed
    {
        public UploadSpeed()
        {
            Name = "Upload Speed";
        }
    }
}