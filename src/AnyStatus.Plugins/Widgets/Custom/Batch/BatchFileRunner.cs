using AnyStatus.API;
using System;
using System.Diagnostics;
using System.IO;

namespace AnyStatus
{
    public class BatchFileRunner : RequestHandler<HealthCheckRequest<BatchFile>>,
        ICheckHealth<BatchFile>
    {
        private readonly IProcessStarter _processStarter;

        public BatchFileRunner(IProcessStarter processStarter)
        {
            _processStarter = processStarter ?? throw new ArgumentNullException();
        }

        [DebuggerStepThrough]
        protected override void HandleCore(HealthCheckRequest<BatchFile> request)
        {
            if (!File.Exists(request.DataContext.FileName))
                throw new FileNotFoundException(request.DataContext.FileName);

            var info = new ProcessStartInfo
            {
                FileName = request.DataContext.FileName,
                Arguments = request.DataContext.Arguments,
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var exitCode = _processStarter.Start(info, TimeSpan.FromMinutes(request.DataContext.Timeout));

            request.DataContext.State = exitCode == request.DataContext.ExitCode ? State.Ok : State.Failed;
        }
    }
}