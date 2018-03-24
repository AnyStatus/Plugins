using AnyStatus.API;
using System;
using System.Diagnostics;
using System.IO;

namespace AnyStatus
{
    public class PowerShellRunner : RequestHandler<HealthCheckRequest<PowerShellScript>>,
        ICheckHealth<PowerShellScript>
    {
        private readonly IProcessStarter _processStarter;

        public PowerShellRunner(IProcessStarter processStarter)
        {
            _processStarter = processStarter ?? throw new ArgumentNullException();
        }

        [DebuggerStepThrough]
        protected override void HandleCore(HealthCheckRequest<PowerShellScript> request)
        {
            if (!File.Exists(request.DataContext.FileName))
                throw new FileNotFoundException(request.DataContext.FileName);

            var executionPolicy = request.DataContext.BypassExecutionPolicy ? "ByPass" : "Restricted";

            var info = new ProcessStartInfo
            {
                FileName = "PowerShell.exe",
                Arguments = $"-ExecutionPolicy {executionPolicy} -File \"{request.DataContext.FileName}\" {request.DataContext.Arguments}",
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var exitCode = _processStarter.Start(info, TimeSpan.FromMinutes(request.DataContext.Timeout));

            request.DataContext.State = exitCode == 0 ? State.Ok : State.Failed;
        }
    }
}