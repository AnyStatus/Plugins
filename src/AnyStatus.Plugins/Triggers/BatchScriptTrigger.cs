using AnyStatus.API;
using AnyStatus.API.Triggers;

namespace AnyStatus
{
    public class BatchScriptTriggerHandler : StartProcessHandler<BatchScriptTrigger>
    {
        public BatchScriptTriggerHandler(IProcessStarter processStarter) : base(processStarter) { }

        protected override void HandleCore(BatchScriptTrigger request)
        {
            var args = GetArgs(request);

            StartProcess("cmd.exe", $"/c \"{request.FileName}\" {args}", request.WorkingDirectory);
        }
    }
}
