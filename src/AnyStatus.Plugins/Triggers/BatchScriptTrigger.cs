using System;
using System.Collections.Generic;
using AnyStatus.API;
using AnyStatus.API.Triggers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class BatchScriptTriggerHandler : StartProcessHandler, IRequestHandler<BatchScriptTrigger>
    {
        public BatchScriptTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        public Task Handle(BatchScriptTrigger request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException();

            var args = request.Arguments;

            if (string.IsNullOrEmpty(args)) goto StartProcess;

            var tokens = new Dictionary<string, string> {
                { "{transitionFrom}", request.OldState.ToString() },
                { "{transitionTo}", request.NewState.ToString() },
            };

            foreach (var kv in tokens)
                args = args.Replace(kv.Key, kv.Value);

            StartProcess: return StartProcess("cmd.exe", $"/c \"{request.FileName}\" {args}", request.WorkingDirectory);
        }
    }
}
