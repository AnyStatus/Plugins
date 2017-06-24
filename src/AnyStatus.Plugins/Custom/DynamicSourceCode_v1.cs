using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

//http://trelford.com/blog/post/C-Scripting.aspx
//http://stackoverflow.com/questions/1799373/how-can-i-prevent-compileassemblyfromsource-from-leaking-memory?noredirect=1&lq=1
//https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/main-and-command-args/

namespace AnyStatus
{
    [DisplayColumn("Custom")]
    [DisplayName("Dynamic Source Code")]
    [Description("Experimental. Create your own custom monitor using C# or VB.NET file.")]
    public class DynamicSourceCode_v1 : Plugin, IAmMonitored
    {
        private const string Category = "Dynamic Source Code";

        public DynamicSourceCode_v1()
        {
            References = new List<string>() { "mscorlib.dll", "System.dll", "System.Configuration.dll", "System.Core.dll", "System.Xml.dll" };
            Arguments = new List<string>();
        }

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("File Name")]
        [Description("C# or VB.NET file path.\n" +
            "Note, the file must have a static Main() method inside a class or a struct (similar to console applications). " +
            "Main can either have a void or int return type to control the status of the monitor. " +
            "The Main method can be declared with or without a string[] parameter that contains the arguments.")]
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
            get { return References.ToArray(); }
            set { References = value.ToList(); }
        }

        [XmlIgnore]
        [PropertyOrder(30)]
        [Category(Category)]
        [Description("Optional. Add arguments to the script invocation. The arguments are passed to the Main(string[] args) method if a string[] parameter exists.")]
        public List<string> Arguments { get; set; }

        [Browsable(false)]
        public string[] ArgumentsArray
        {
            get { return Arguments.ToArray(); }
            set { Arguments = value.ToList(); }
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

    public class CSharpFileMonitor : IMonitor<DynamicSourceCode_v1>
    {
        private readonly ILogger _logger;
        private readonly Func<ICompiler> _compilerFactory;

        public CSharpFileMonitor(ILogger logger, Func<ICompiler> compilerFactory)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _compilerFactory = Preconditions.CheckNotNull(compilerFactory, nameof(compilerFactory));
        }

        [DebuggerStepThrough]
        public void Handle(DynamicSourceCode_v1 item)
        {
            var sourceFile = new FileInfo(item.FileName);

            if (!sourceFile.Exists)
                throw new FileNotFoundException(sourceFile.FullName);

            if (item.CompiledAssembly == null || item.LastWriteTime != sourceFile.LastWriteTime)
                Compile(item, sourceFile);

            var result = Run(item.CompiledAssembly, item.Arguments);

            item.State = result is int ? State.FromValue((int)result) : State.Ok;
        }

        [DebuggerStepThrough]
        private void Compile(DynamicSourceCode_v1 item, FileInfo sourceFile)
        {
            var compiler = _compilerFactory();

            item.CompiledAssembly = compiler.Compile(sourceFile, item.References, item.TreatWarningsAsErrors);

            item.LastWriteTime = sourceFile.LastWriteTime;

            _logger.Info($"Source {sourceFile.FullName} built successfully.");
        }

        /// <summary>
        /// Runs an executable assembly.
        /// The assembly must have an entry point.
        /// </summary>
        /// <returns>Null or state id</returns>
        private static object Run(Assembly assembly, IEnumerable<string> args)
        {
            try
            {
                var method = assembly.EntryPoint;

                var parameters = method.GetParameters();

                if (parameters.Any())
                {
                    if (parameters.First().ParameterType != typeof(string[]))
                        throw new Exception("The assembly entry point method first parameter must be of type string[].");

                    return method.Invoke(null, new object[] { args != null ? args.ToArray() : new string[0] });
                }
                else
                {
                    return method.Invoke(null, new object[] { });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing CSharp code.", ex);
            }
        }
    }
}
