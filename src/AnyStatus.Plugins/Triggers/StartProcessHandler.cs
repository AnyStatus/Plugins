using AnyStatus.API;
using AnyStatus.API.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnyStatus
{
    public abstract class StartProcessHandler<T> : RequestHandler<T> where T : CommandTrigger
    {
        private readonly IProcessStarter _processStarter;

        protected StartProcessHandler(IProcessStarter processStarter)
        {
            _processStarter = processStarter ?? throw new ArgumentNullException();
        }

        protected override void HandleCore(T request) => throw new NotImplementedException();

        protected string GetArgs(T request)
        {
            var args = request.Arguments;

            var values = new Dictionary<string, string> {
                { "{transitionFrom}", request.OldState.ToString() },
                { "{transitionTo}", request.NewState.ToString() },
            };

            foreach (var key in values.Keys)
                args = args.Replace(key, values[key]);

            return args;
        }

        protected void StartProcess(string fileName, string args, string workingDirectory)
        {
            var info = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            _processStarter.Start(info, TimeSpan.FromMinutes(10));
        }
    }
}
