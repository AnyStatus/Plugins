using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

//http://trelford.com/blog/post/C-Scripting.aspx
//http://stackoverflow.com/questions/1799373/how-can-i-prevent-compileassemblyfromsource-from-leaking-memory?noredirect=1&lq=1
//https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/main-and-command-args/

namespace AnyStatus
{
    [DisplayColumn("Code")]
    [DisplayName(".NET Source Code")]
    [Description("Compile and execute C# or VB.NET source code at runtime. The status can be set in the source code. AnyStatus will change the status to Failed if the source code throws an exception.")]
    public class DynamicSourceCode_v1 : Widget, ISchedulable, IHealthCheck
    {
        private const string Category = "Source Code";

        public DynamicSourceCode_v1()
        {
            Arguments = new List<string>();
            References = new List<string>
            {
                "mscorlib.dll",
                "System.dll",
                "System.Configuration.dll",
                "System.Core.dll",
                "System.Xml.dll"
            };
        }

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("File Name")]
        [Description("C# or VB.NET source code path.\n" +
            "The file must have a static Main() method inside a class or a struct (similar to a console application). " +
            "Main() method should return either void or int to change the status of the widget. " +
            "Main() method can accept a string[] array in the function parameters to pass the arguments to the source code. For example: Main(string[] args).")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [XmlIgnore]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Assembly References")]
        [Description("Optional. Add assembly references by specifying assembly file name or full path.")]
        public List<string> References { get; set; }

        [Browsable(false)]
        public string[] ReferencesArray
        {
            get => References.ToArray();
            set => References = value.ToList();
        }

        [XmlIgnore]
        [PropertyOrder(30)]
        [Category(Category)]
        [Description("Optional. Add arguments to the script invocation. The arguments are passed to the Main(string[] args) method if a string[] parameter exists.")]
        public List<string> Arguments { get; set; }

        [Browsable(false)]
        public string[] ArgumentsArray
        {
            get => Arguments.ToArray();
            set => Arguments = value.ToList();
        }

        [PropertyOrder(40)]
        [Category(Category)]
        [DisplayName("Treat warnings as errors")]
        [Description("")]
        public bool TreatWarningsAsErrors { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public Assembly CompiledAssembly { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public DateTime LastWriteTime { get; set; }
    }
}