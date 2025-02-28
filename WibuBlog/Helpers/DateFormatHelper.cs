namespace WibuBlog.Helpers
{
    public static class DateFormatHelper
    {
        public static string GetDateFormat(DateTime date)
        {
            var timeDifference = DateTime.Now.Subtract(date);
            string timeAgo;

            if (timeDifference.TotalDays >= 1)
            {
                timeAgo = $"{(int)timeDifference.TotalDays} days ago";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                timeAgo = $"{(int)timeDifference.TotalHours} hours ago";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                timeAgo = $"{(int)timeDifference.TotalMinutes} minutes ago";
            }
            else
            {
                timeAgo = "Just now";
            }
            return timeAgo;
        }
    }
}
