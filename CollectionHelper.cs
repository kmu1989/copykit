using System;
using System.Collections.Generic;

namespace Common.Helpers
{
    /// <summary>
    /// 컬렉션/딕셔너리 관련 범용 헬퍼.
    /// 모든 메서드는 static 이며 BCL 외 의존성이 없어 함수 단위로 복사해 사용할 수 있습니다.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// null 이거나 비어 있는 컬렉션인지 검사.
        /// </summary>
        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// null 이거나 비어 있는 딕셔너리인지 검사.
        /// </summary>
        public static bool IsNullOrEmpty<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            return dict == null || dict.Count == 0;
        }

        /// <summary>
        /// 컬렉션을 size 단위로 분할. (대량 데이터 배치 처리, IN 절 분할 등에 유용)
        /// 예: Chunk([1,2,3,4,5], 2) => [[1,2],[3,4],[5]]
        /// </summary>
        public static List<List<T>> Chunk<T>(IEnumerable<T> source, int size)
        {
            var result = new List<List<T>>();
            if (source == null) return result;
            if (size <= 0) throw new ArgumentOutOfRangeException("size", "size는 1 이상이어야 합니다.");

            var bucket = new List<T>(size);
            foreach (var item in source)
            {
                bucket.Add(item);
                if (bucket.Count == size)
                {
                    result.Add(bucket);
                    bucket = new List<T>(size);
                }
            }
            if (bucket.Count > 0) result.Add(bucket);
            return result;
        }

        /// <summary>
        /// 키가 있으면 값을, 없으면 defaultValue 반환. (예외 없는 딕셔너리 조회)
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(
            IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if (dict == null) return defaultValue;
            TValue value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// 키 기준 중복 제거. .NET 6 미만 환경에서 LINQ DistinctBy 대체.
        /// 첫 등장 항목을 유지하며 지연 평가(yield)합니다.
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null) yield break;
            var seen = new HashSet<TKey>();
            foreach (var item in source)
            {
                if (seen.Add(keySelector(item)))
                    yield return item;
            }
        }

        /// <summary>
        /// 키로 그룹화하여 Dictionary 생성. 같은 키는 리스트로 누적.
        /// </summary>
        public static Dictionary<TKey, List<T>> GroupToDictionary<T, TKey>(
            IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            var result = new Dictionary<TKey, List<T>>();
            if (source == null) return result;

            foreach (var item in source)
            {
                TKey key = keySelector(item);
                List<T> bucket;
                if (!result.TryGetValue(key, out bucket))
                {
                    bucket = new List<T>();
                    result.Add(key, bucket);
                }
                bucket.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 컬렉션이 null 이어도 안전하게 순회. null 이면 빈 시퀀스로 취급.
        /// 예: foreach (var x in CollectionHelper.OrEmpty(maybeNullList)) { ... }
        /// </summary>
        public static IEnumerable<T> OrEmpty<T>(IEnumerable<T> source)
        {
            return source ?? new T[0];
        }
    }
}
