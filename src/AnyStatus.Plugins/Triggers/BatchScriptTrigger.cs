using AnyStatus.API;
using AnyStatus.API.Triggers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class BatchScriptTriggerHandler : BaseCommandTriggerHandler, IRequestHandler<BatchScriptTrigger>
    {
        public BatchScriptTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        public Task Handle(BatchScriptTrigger request, CancellationToken cancellationToken)
        {
            var args = GetArgs(request);

            return StartProcess("cmd.exe", $"/c \"{request.FileName}\" {args}", request.WorkingDirectory);
        }
    }
}
