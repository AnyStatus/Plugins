﻿using AnyStatus.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.NuGet
{
    [DisplayColumn("NuGet")]
    [DisplayName("NuGet Package")]
    [Description("Get NuGet package information.")]
    public class NuGetPackageWidget : Metric, ISchedulable, IWebPage
    {
        public NuGetPackageWidget()
        {
            ShowErrorNotifications = false;
        }

        [Category("NuGet")]
        [DisplayName("Package Id")]
        [Description("The NuGet package id. For example: AnyStatus.API")]
        public string PackageId { get; set; }

        [Category("NuGet")]
        [DisplayName("Package Source")]
        [Description("The NuGet server feed URL. Official NuGet package source: https://api.nuget.org/v3/index.json")]
        public string PackageSource { get; set; } = "https://api.nuget.org/v3/index.json";

        [Category("NuGet")]
        [DisplayName("Pre-Release")]
        [Description("Enable to include pre-release packages.")]
        public bool PreRelease { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://www.nuget.org/packages/{PackageId}/";
    }
}
