using System;
using System.Collections.Generic;
using AnyStatus.API;
using AnyStatus.API.Triggers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class CommandTriggerHandler : StartProcessHandler, IRequestHandler<CommandTrigger>
    {
        public CommandTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        public Task Handle(CommandTrigger request, CancellationToken cancellationToken)
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

            StartProcess: return StartProcess(request.FileName, args, request.WorkingDirectory);
        }
    }
}
