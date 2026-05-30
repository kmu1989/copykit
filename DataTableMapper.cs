using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Common.Helpers
{
    /// <summary>
    /// DataTable / DataRow 를 모델 클래스로 매핑하는 헬퍼.
    ///
    /// 매칭 규칙: 컬럼명과 프로퍼티명이 "정확히 일치"해야 매핑됩니다.
    ///           (대소문자 무시 — DataTable.Columns 기본 동작)
    ///           이름이 다르면 매핑되지 않으므로, 쿼리 별칭(AS) 또는 프로퍼티명을 맞추세요.
    ///
    /// 처리: DBNull, Nullable(int? 등), Enum, Guid 변환.
    /// 외부 라이브러리(Dapper 등) 없이 BCL Reflection 만 사용.
    ///
    /// 복사 시 주의: ToList/ToModel 은 내부 private 메서드(ConvertValue)에 의존합니다.
    ///              떼어갈 때 이 클래스 전체를 가져가세요.
    /// </summary>
    public static class DataTableMapper
    {
        /// <summary>
        /// DataTable -> List&lt;T&gt;. T 는 매개변수 없는 생성자가 필요합니다.
        /// </summary>
        public static List<T> ToList<T>(DataTable table) where T : new()
        {
            var result = new List<T>();
            if (table == null || table.Rows.Count == 0) return result;

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRow<T>(row, props));
            }
            return result;
        }

        /// <summary>
        /// DataRow 한 건 -> 모델 객체.
        /// </summary>
        public static T ToModel<T>(DataRow row) where T : new()
        {
            if (row == null) return new T();
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return MapRow<T>(row, props);
        }

        // ----- 내부 구현 -----

        private static T MapRow<T>(DataRow row, PropertyInfo[] props) where T : new()
        {
            var item = new T();
            DataColumnCollection columns = row.Table.Columns;

            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite) continue;
                if (!columns.Contains(prop.Name)) continue; // 정확 매칭 (대소문자 무시)

                object raw = row[prop.Name];
                if (raw == null || raw == DBNull.Value) continue; // 기본값 유지

                object converted;
                if (ConvertValue(raw, prop.PropertyType, out converted))
                {
                    prop.SetValue(item, converted, null);
                }
                // 변환 실패 시: 기본값 유지(예외 없음). 엄격 모드가 필요하면 여기서 throw 하세요.
            }
            return item;
        }

        /// <summary>
        /// raw 값을 targetType 으로 변환. 성공 여부를 반환.
        /// Nullable, Enum, Guid, 일반 IConvertible 을 처리.
        /// </summary>
        private static bool ConvertValue(object raw, Type targetType, out object converted)
        {
            converted = null;
            try
            {
                Type underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlying == typeof(string))
                {
                    converted = raw.ToString();
                }
                else if (underlying.IsEnum)
                {
                    converted = raw is string
                        ? Enum.Parse(underlying, (string)raw, true)
                        : Enum.ToObject(underlying, raw);
                }
                else if (underlying == typeof(Guid))
                {
                    converted = raw is Guid ? raw : Guid.Parse(raw.ToString());
                }
                else if (underlying.IsAssignableFrom(raw.GetType()))
                {
                    converted = raw; // 이미 호환 타입
                }
                else
                {
                    converted = Convert.ChangeType(raw, underlying);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
