using AnyStatus.API;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnyStatus
{
    public abstract class StartProcessHandler
    {
        private readonly IProcessStarter _processStarter;

        protected StartProcessHandler(IProcessStarter processStarter)
        {
            _processStarter = processStarter ?? throw new ArgumentNullException();
        }

        protected Task StartProcess(string fileName, string args, string workingDirectory)
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

            return Task.CompletedTask;
        }
    }
}
