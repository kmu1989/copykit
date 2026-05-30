# Common.Helpers

사내 표준 C# 범용 헬퍼 모음. **함수 단위 복사 사용**을 전제로 설계했습니다.

- 모든 메서드는 `public static` — 인스턴스 생성/필드 의존 없음
- BCL(기본 클래스 라이브러리) 외 외부 의존성 0 — 폐쇄망 / 버전 고정 환경에서도 그대로 복붙 가능
- C# 7.3 / .NET Framework 4.6.1+ / .NET Standard 2.0 호환 (nullable 어노테이션·최신 문법 미사용)
- 네임스페이스 `Common.Helpers` 는 프로젝트에 맞게 바꿔 쓰세요.

> 사용법: 필요한 함수만 골라 복사하세요. 함수 위 주석이 그대로 따라가 설명이 됩니다.
> 단, `DataTableMapper` 는 내부 private 메서드에 의존하므로 **클래스 전체**를 가져가세요.

---

## StringHelper.cs

| 함수 | 설명 |
|---|---|
| `IsBlank(value)` | null/빈/공백 문자열 검사 |
| `HasText(value)` | 내용 있는 문자열 검사 (IsBlank 반대) |
| `DefaultIfBlank(value, defaultValue)` | 비어 있으면 기본값 |
| `SafeSubstring(value, start, length)` | 범위 초과해도 예외 없는 Substring |
| `Left(value, length)` / `Right(value, length)` | 좌/우에서 n글자 |
| `Truncate(value, maxLength, suffix="...")` | 길면 자르고 말줄임 |
| `EqualsIgnoreCase(a, b)` | 대소문자 무시 비교 |
| `ContainsIgnoreCase(source, value)` | 대소문자 무시 포함 |
| `OnlyDigits(value)` | 숫자만 추출 (예: 전화번호) |
| `Mask(value, prefix, suffix, maskChar='*')` | 가운데 마스킹 (개인정보 표시) |
| `ToInt / ToLong / ToDecimal(value, default)` | 안전한 숫자 변환 |

## DateHelper.cs

| 함수 | 설명 |
|---|---|
| `StartOfDay(date)` / `EndOfDay(date)` | 그날의 시작/끝 (범위 조회 경계) |
| `StartOfMonth(date)` / `EndOfMonth(date)` | 그달의 시작/끝 |
| `ToDateString` / `ToDateTimeString` / `ToCompactDate` | yyyy-MM-dd / yyyy-MM-dd HH:mm:ss / yyyyMMdd |
| `TryParse(value, params formats)` | 안전 파싱, 실패 시 null |
| `Age(birthDate, asOf=null)` | 만 나이 |
| `IsBetween(target, start, end)` | 구간 포함 검사 (자동 정렬) |
| `DaysBetween(a, b)` | 일수 차(절댓값) |
| `ToUnixSeconds` / `FromUnixSeconds` | Unix epoch(초) 변환 |

## CollectionHelper.cs

| 함수 | 설명 |
|---|---|
| `IsNullOrEmpty(collection)` | 컬렉션 null/빈 검사 |
| `IsNullOrEmpty(dict)` | 딕셔너리 null/빈 검사 |
| `Chunk(source, size)` | n개 단위 분할 (배치 처리, IN절 분할) |
| `GetOrDefault(dict, key, default)` | 예외 없는 딕셔너리 조회 |
| `DistinctBy(source, keySelector)` | 키 기준 중복 제거 (.NET6 미만 대체) |
| `GroupToDictionary(source, keySelector)` | 키별 그룹화 |
| `OrEmpty(source)` | null이어도 안전하게 순회 |

## DataTableMapper.cs

| 함수 | 설명 |
|---|---|
| `ToList<T>(table)` | DataTable → List&lt;T&gt; |
| `ToModel<T>(row)` | DataRow → 모델 1건 |

- **정확 매칭**: 컬럼명 = 프로퍼티명 정확히 일치해야 매핑 (대소문자만 무시). 이름이 다르면 쿼리 별칭(`AS`)으로 맞추세요.
- DBNull, `int?` 등 Nullable, Enum, Guid 변환 처리
- 변환 실패 시 예외 없이 기본값 유지 (엄격 모드가 필요하면 `MapRow` 주석 위치에서 throw)

```csharp
// 사용 예
DataTable dt = /* ... 쿼리 결과 ... */;
List<UserModel> users = DataTableMapper.ToList<UserModel>(dt);
```

---

## License

MIT (공개 시). 사내 도메인 로직·시스템 전제가 섞이지 않은 순수 범용 코드만 포함하세요.
