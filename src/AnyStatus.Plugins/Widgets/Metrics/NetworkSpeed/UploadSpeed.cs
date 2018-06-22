using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Upload Speed")]
    [DisplayColumn("Metrics")]
    [Description("Monitor network upload speed.")]
    public class UploadSpeed : NetworkSpeed
    {
    }
}