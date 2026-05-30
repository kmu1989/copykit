using System;
using System.Text;

namespace Common.Helpers
{
    /// <summary>
    /// 문자열 관련 범용 헬퍼.
    /// 모든 메서드는 static 이며 BCL 외 의존성이 없어 함수 단위로 복사해 사용할 수 있습니다.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// null / 빈 문자열 / 공백만 있는 문자열인지 검사. (string.IsNullOrWhiteSpace 의도 명확화 래핑)
        /// </summary>
        public static bool IsBlank(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 실제 내용이 있는 문자열인지 검사. IsBlank 의 반대.
        /// </summary>
        public static bool HasText(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 값이 비어 있으면 기본값 반환.
        /// 예: DefaultIfBlank(null, "-") => "-"
        /// </summary>
        public static string DefaultIfBlank(string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        /// <summary>
        /// 범위를 벗어나도 예외 없이 잘라 반환하는 안전한 Substring.
        /// 예: SafeSubstring("abc", 1, 10) => "bc"
        /// </summary>
        public static string SafeSubstring(string value, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (startIndex < 0) startIndex = 0;
            if (startIndex >= value.Length) return string.Empty;

            int available = value.Length - startIndex;
            int take = Math.Min(length, available);
            return take <= 0 ? string.Empty : value.Substring(startIndex, take);
        }

        /// <summary>
        /// 왼쪽에서 length 글자. 문자열보다 길면 전체 반환.
        /// </summary>
        public static string Left(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || length <= 0) return string.Empty;
            return length >= value.Length ? value : value.Substring(0, length);
        }

        /// <summary>
        /// 오른쪽에서 length 글자. 문자열보다 길면 전체 반환.
        /// </summary>
        public static string Right(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || length <= 0) return string.Empty;
            return length >= value.Length ? value : value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// maxLength 초과 시 자르고 suffix(기본 "...") 를 붙임.
        /// 예: Truncate("안녕하세요 반갑습니다", 5) => "안녕하세요..."
        /// </summary>
        public static string Truncate(string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value ?? string.Empty;
            if (maxLength <= 0) return string.Empty;
            return value.Substring(0, maxLength) + (suffix ?? string.Empty);
        }

        /// <summary>
        /// 대소문자 무시 비교. null 끼리도 true.
        /// </summary>
        public static bool EqualsIgnoreCase(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 대소문자 무시 포함 여부.
        /// </summary>
        public static bool ContainsIgnoreCase(string source, string value)
        {
            if (source == null || value == null) return false;
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 숫자만 추출. 예: OnlyDigits("010-1234-5678") => "01012345678"
        /// </summary>
        public static string OnlyDigits(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (c >= '0' && c <= '9') sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 앞/뒤 일부만 남기고 가운데를 마스킹. 개인정보 표시 등에 사용.
        /// 예: Mask("01012345678", 3, 4) => "010****5678"
        /// </summary>
        public static string Mask(string value, int visiblePrefix, int visibleSuffix, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (visiblePrefix < 0) visiblePrefix = 0;
            if (visibleSuffix < 0) visibleSuffix = 0;
            if (visiblePrefix + visibleSuffix >= value.Length)
                return value; // 가릴 부분이 없음

            int maskCount = value.Length - visiblePrefix - visibleSuffix;
            return value.Substring(0, visiblePrefix)
                   + new string(maskChar, maskCount)
                   + value.Substring(value.Length - visibleSuffix);
        }

        /// <summary>
        /// 안전한 int 변환. 실패 시 defaultValue 반환.
        /// </summary>
        public static int ToInt(string value, int defaultValue = 0)
        {
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 안전한 long 변환. 실패 시 defaultValue 반환.
        /// </summary>
        public static long ToLong(string value, long defaultValue = 0)
        {
            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 안전한 decimal 변환. 실패 시 defaultValue 반환.
        /// </summary>
        public static decimal ToDecimal(string value, decimal defaultValue = 0)
        {
            decimal result;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }
    }
}
