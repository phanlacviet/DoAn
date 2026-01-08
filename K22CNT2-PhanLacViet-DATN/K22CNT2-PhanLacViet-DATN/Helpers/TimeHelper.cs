namespace K22CNT2_PhanLacViet_DATN.Helpers
{
    public static class TimeHelper
    {
        public static string GetTimeAgo(DateTime? date)
        {
            if (!date.HasValue) return "N/A";

            var timeSpan = DateTime.Now - date.Value;

            if (timeSpan.TotalSeconds < 0) return "Vừa xong";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";

            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";

            if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"{months} tháng trước";
            }

            int years = (int)(timeSpan.TotalDays / 365);
            return $"{years} năm trước";
        }
    }
}
