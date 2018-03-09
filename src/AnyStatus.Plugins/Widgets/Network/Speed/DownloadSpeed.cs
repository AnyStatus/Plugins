using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Download Speed")]
    [DisplayColumn("Network")]
    [Description("Monitor network download speed.")]
    public class DownloadSpeed : NetworkSpeed
    {
    }
}