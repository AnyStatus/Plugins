using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus
{
    [DisplayName("Download Speed")]
    [DisplayColumn("System Information")]
    [Description("Monitor network download speed.")]
    public class DownloadSpeed : NetworkSpeed
    {
        public DownloadSpeed()
        {
            Name = "Download Speed";
        }
    }
}