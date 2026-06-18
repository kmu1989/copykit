using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Common.Helpers
{
    /// <summary>
    /// DataTable 을 모델 클래스 리스트로 매핑하는 헬퍼.
    ///
    /// 매칭 규칙: 컬럼명과 프로퍼티명이 "정확히 일치"해야 매핑됩니다.
    ///           (대소문자 무시 — StringComparer.OrdinalIgnoreCase)
    ///           이름이 다르면 매핑되지 않으므로, 쿼리 별칭(AS) 또는 프로퍼티명을 맞추세요.
    ///
    /// 처리: DBNull, Nullable(int? 등), Enum 변환.
    /// 외부 라이브러리(Dapper 등) 없이 BCL Reflection 만 사용.
    ///
    /// 사용 예제:
    /// <code>
    /// // 1) 모델 정의 — 프로퍼티명을 컬럼명과 맞춥니다.
    /// public class User
    /// {
    ///     public int      Id     { get; set; }
    ///     public string   Name   { get; set; }
    ///     public int?     Age    { get; set; }   // Nullable 지원
    ///     public Gender   Gender { get; set; }   // Enum 지원
    /// }
    ///
    /// // 2) 호출 — DataTable 을 List&lt;User&gt; 로 변환
    /// DataTable dt = dataAccess.GetUsers();          // SELECT Id, Name, Age, Gender FROM Users
    /// List&lt;User&gt; users = DataTableMapper.ConvertToList&lt;User&gt;(dt);
    ///
    /// // 3) 변환 후 활용 (LINQ)
    /// User first   = users.FirstOrDefault();                       // 첫 행
    /// User byId    = users.Find(u =&gt; u.Id == 100);                 // 단건 조회
    /// List&lt;User&gt; adults = users.FindAll(u =&gt; u.Age &gt;= 18);         // 조건 필터(여러 건)
    /// List&lt;User&gt; sorted = users.OrderBy(u =&gt; u.Name).ToList();      // 정렬
    /// int count = users.Count;                                      // 건수
    /// </code>
    /// </summary>
    public static class DataTableMapper
    {
        /// <summary>
        /// DataTable -> List&lt;T&gt;. T 는 매개변수 없는 생성자가 필요합니다.
        /// </summary>
        public static List<T> ConvertToList<T>(DataTable dt) where T : new()
        {
            var list = new List<T>();

            if (dt == null || dt.Rows.Count == 0)
                return list;

            var properties = typeof(T).GetProperties();

            // .NET Framework 4.5 호환 버전
            var columns = new HashSet<string>(
                dt.Columns
                  .Cast<DataColumn>()
                  .Select(c => c.ColumnName),
                StringComparer.OrdinalIgnoreCase);

            // .NET Framework 4.7.2+ / .NET Core(Standard 2.1) 버전
            // (ToHashSet 확장 메서드는 4.7.2 이상에서만 제공)
            //var columns = dt.Columns
            //                .Cast<DataColumn>()
            //                .Select(c => c.ColumnName)
            //                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in dt.Rows)
            {
                var obj = new T();

                foreach (var prop in properties)
                {
                    if (!prop.CanWrite)
                        continue;

                    if (!columns.Contains(prop.Name))
                        continue;

                    var value = row[prop.Name];

                    if (value == DBNull.Value)
                        continue;

                    var targetType =
                        Nullable.GetUnderlyingType(prop.PropertyType)
                        ?? prop.PropertyType;

                    object convertedValue;

                    if (targetType.IsEnum)
                    {
                        convertedValue = Enum.Parse(
                            targetType,
                            value.ToString(),
                            true);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(
                            value,
                            targetType);
                    }

                    prop.SetValue(obj, convertedValue, null);
                }

                list.Add(obj);
            }

            return list;
        }
    }
}
