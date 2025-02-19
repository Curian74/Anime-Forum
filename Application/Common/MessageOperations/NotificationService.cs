using System.Collections.Generic;

namespace Application.Common.MessageOperations
{
    public static class NotificationService
    {
        private static readonly Dictionary<string, string> _notifications = new()
        {
            { "NOTI01", "Your post <b>{0}</b> has been uploaded on <i>{1}</i>." },
            { "NOTI02", "Your post <b>{0}</b> has been edited on <i>{1}</i>." },
            { "NOTI03", "Your post <b>{0}</b> has been deleted on <i>{1}</i>." },
            { "NOTI06", "You have been <span style='color: red;'>banned</span> from <b>{0}</b> for: <i>{1}</i> until <b>{2}</b>.<br><small>Date: {3}</small>" },
            { "NOTI07", "Your post has been deleted due to <i>{0}</i>, please be cautious with your content.<br><small>Date: {1}</small>" }
        };

        public static string GetNotification(string code, params object[] args)
        {
            if (_notifications.TryGetValue(code, out string? template))
            {
                return string.Format(template, args);
            }
            return "<b style='color: red;'>Notification not found</b>";
        }

        //string notificationHtml = NotificationService.GetNotificationHtml("NOTI06", "My System", "Spam", "20/02/2025", "12/02/2025");
    }
}
