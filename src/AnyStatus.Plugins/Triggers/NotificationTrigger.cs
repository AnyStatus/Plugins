using AnyStatus.API;
using AnyStatus.API.Triggers;

namespace AnyStatus
{
    public class NotificationTriggerHandler : RequestHandler<NotificationTrigger>
    {
        private readonly INotificationService _notificationService;

        public NotificationTriggerHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        protected override void HandleCore(NotificationTrigger request)
        {
            _notificationService.TrySend(new Notification(request.Message, request.Icon));
        }
    }
}
