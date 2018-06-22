using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayColumn("Metrics")]
    [DisplayName("Download Speed")]
    [Description("Monitor network download speed.")]
    public class DownloadSpeed : NetworkSpeed
    {
    }
}