namespace Application.Common.MessageOperations
{
    public static class NotificationService
    {
        private static readonly Dictionary<string, string> _notifications = new()
        {
            { "NOTI01", "Your post <b>{0}</b> has been uploaded on <i>{1}</i>." },
            { "NOTI02", "Your post <b>{0}</b> has been edited on <i>{1}</i>." },
            { "NOTI03", "Your post <b>{0}</b> has been deleted on <i>{1}</i>." },
            { "NOTI04","{0} has commented on your post <i>{1}</i>"},
            { "NOTI06", "You have been <span style='color: red;'>banned</span> from <b>{0}</b> for: <i>{1}</i> until <b>{2}</b>.<br><small>Date: {3}</small>" },
            { "NOTI07", "Your post has been deleted due to <i>{0}</i>, please be cautious with your content.<br><small>Date: {1}</small>" },                       
            { "NOTIN01", "You've successfully updated your profile picture" },
            { "NOTIN02", "You've receive an upvote on post {0}"},
            { "NOTIN03", "You've receive a downvote on post {0}"},
            { "NOTIN04", "You've been promoted to Moderator"},
            { "NOTIN05", "You've been demoted to Member"},
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
