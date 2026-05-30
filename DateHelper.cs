using System;

namespace Common.Helpers
{
    /// <summary>
    /// 날짜/시간 관련 범용 헬퍼.
    /// 모든 메서드는 static 이며 BCL 외 의존성이 없어 함수 단위로 복사해 사용할 수 있습니다.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// 해당 날짜의 시작 시각(00:00:00.000).
        /// </summary>
        public static DateTime StartOfDay(DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        /// 해당 날짜의 끝 시각(23:59:59.9999999).
        /// 범위 조회 시 종료 경계로 유용.
        /// </summary>
        public static DateTime EndOfDay(DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 해당 월의 1일 00:00:00.
        /// </summary>
        public static DateTime StartOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 해당 월의 마지막 날 23:59:59.9999999.
        /// </summary>
        public static DateTime EndOfMonth(DateTime date)
        {
            return StartOfMonth(date).AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        /// "yyyy-MM-dd" 형식 문자열. null 가능성 없는 DateTime 전용.
        /// </summary>
        public static string ToDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// "yyyy-MM-dd HH:mm:ss" 형식 문자열.
        /// </summary>
        public static string ToDateTimeString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// "yyyyMMdd" 형식. (DB 키나 파일명에 자주 사용)
        /// </summary>
        public static string ToCompactDate(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 안전한 날짜 파싱. 실패 시 null 반환.
        /// 여러 포맷을 순서대로 시도.
        /// </summary>
        public static DateTime? TryParse(string value, params string[] formats)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (formats != null && formats.Length > 0)
            {
                DateTime exact;
                if (DateTime.TryParseExact(value, formats,
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out exact))
                {
                    return exact;
                }
                return null;
            }

            DateTime result;
            return DateTime.TryParse(value, out result) ? (DateTime?)result : null;
        }

        /// <summary>
        /// 만 나이 계산. asOf 미지정 시 오늘 기준.
        /// </summary>
        public static int Age(DateTime birthDate, DateTime? asOf = null)
        {
            DateTime today = (asOf ?? DateTime.Today).Date;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }

        /// <summary>
        /// target 이 [start, end] 구간 안에 있는지(경계 포함).
        /// </summary>
        public static bool IsBetween(DateTime target, DateTime start, DateTime end)
        {
            if (start > end)
            {
                DateTime tmp = start; start = end; end = tmp; // 자동 정렬
            }
            return target >= start && target <= end;
        }

        /// <summary>
        /// 두 날짜 사이의 일수(절댓값). 시각은 무시하고 날짜 기준.
        /// </summary>
        public static int DaysBetween(DateTime a, DateTime b)
        {
            return Math.Abs((a.Date - b.Date).Days);
        }

        /// <summary>
        /// DateTime -> Unix epoch 초(UTC 기준).
        /// </summary>
        public static long ToUnixSeconds(DateTime dateTime)
        {
            DateTime utc = dateTime.ToUniversalTime();
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(utc - epoch).TotalSeconds;
        }

        /// <summary>
        /// Unix epoch 초 -> DateTime(UTC). 로컬이 필요하면 .ToLocalTime() 추가.
        /// </summary>
        public static DateTime FromUnixSeconds(long seconds)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(seconds);
        }
    }
}
