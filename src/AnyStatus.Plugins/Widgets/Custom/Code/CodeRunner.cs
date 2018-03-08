using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

//http://trelford.com/blog/post/C-Scripting.aspx
//http://stackoverflow.com/questions/1799373/how-can-i-prevent-compileassemblyfromsource-from-leaking-memory?noredirect=1&lq=1
//https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/main-and-command-args/

namespace AnyStatus
{
    public class CodeRunner : RequestHandler<HealthCheckRequest<DynamicSourceCode_v1>>,
        ICheckHealth<DynamicSourceCode_v1>
    {
        private readonly ILogger _logger;
        private readonly Func<ICompiler> _compilerFactory;

        public CodeRunner(ILogger logger, Func<ICompiler> compilerFactory)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _compilerFactory = Preconditions.CheckNotNull(compilerFactory, nameof(compilerFactory));
        }

        [DebuggerStepThrough]
        protected override void HandleCore(HealthCheckRequest<DynamicSourceCode_v1> request)
        {
            var sourceFile = new FileInfo(request.DataContext.FileName);

            if (!sourceFile.Exists)
                throw new FileNotFoundException(sourceFile.FullName);

            if (request.DataContext.CompiledAssembly == null || request.DataContext.LastWriteTime != sourceFile.LastWriteTime)
                Compile(request.DataContext, sourceFile);

            var result = Run(request.DataContext.CompiledAssembly, request.DataContext.Arguments);

            request.DataContext.State = result is int ? State.FromValue((int)result) : State.Ok;
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