using AnyStatus.API;
using AnyStatus.API.Triggers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class CommandTriggerHandler : BaseCommandTriggerHandler, IRequestHandler<CommandTrigger>
    {
        public CommandTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        public Task Handle(CommandTrigger request, CancellationToken cancellationToken)
        {
            var args = GetArgs(request);

            return StartProcess(request.FileName, args, request.WorkingDirectory);
        }
    }
}
