using AnyStatus.API;
using AnyStatus.API.Triggers;
using System.Collections.Generic;

namespace AnyStatus
{
    public class CommandTriggerHandler : StartProcessHandler<CommandTrigger>
    {
        public CommandTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        protected override void HandleCore(CommandTrigger request)
        {
            var args = GetArgs(request);

            StartProcess(request.FileName, args, request.WorkingDirectory);
        }
    }
}
