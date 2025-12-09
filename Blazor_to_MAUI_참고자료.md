# 기존 MapDemo (Blazor) 프로젝트 참고 자료

**작성일**: 2025-12-09  
**목적**: 기존 Blazor Server 프로젝트의 로직을 MAUI 프로젝트로 전환할 때 참고

---

## 📋 1. 참고할 핵심 로직

### ✅ **거리 계산 (Haversine 공식)**

기존 `DistanceCalculator.cs`와 우리 MAUI `LocationService.cs`는 **동일한 로직**입니다.

**차이점**:
- ✅ MAUI: 소수점 2자리 반올림 (`Math.Round(distance, 2)`)
- Blazor: 반올림 없음

**결론**: MAUI 버전이 더 나음 (UI 표시에 적합)

---

### ✅ **Mock 데이터 구조**

#### Blazor 버전
```csharp
// 문자열 ID 사용 (예: "U001", "A001")
public string Id { get; set; } = string.Empty;
```

#### MAUI 버전
```csharp
// 정수 ID 사용 (예: 1, 2, 3...)
public int Id { get; init; }
```

**차이점**:
- Blazor: 문자열 ID (U001~U010, A001~A020)
- MAUI: 정수 ID (1~10, 11~30)

**결론**: 둘 다 괜찮음. MAUI 버전이 더 간결함.

---

### ✅ **좌표 분포 패턴**

#### Blazor의 좌표 분포
```
근거리 (0~1km): 이용자 3명, 지원사 6명
중거리 (1~3km): 이용자 4명, 지원사 10명
원거리 (3~4km): 이용자 3명, 지원사 4명
```

#### MAUI의 좌표 분포
```
근거리 (0~1km): 이용자 3명, 지원사 5명
중거리 (1~2km): 이용자 2명, 지원사 5명
중거리 (2~3km): 이용자 2명, 지원사 5명
원거리 (3~4km): 이용자 3명, 지원사 5명
```

**결론**: MAUI 버전이 더 균등하게 분포 (반경별로 테스트하기 좋음)

---

## 📋 2. 네이버 지도 JS Interop 로직

### ✅ **주요 JavaScript 함수** (Blazor에서 검증된 로직)

| 함수 | 용도 | MAUI 적용 가능 여부 |
|------|------|---------------------|
| `initMap(elementId, lat, lng, zoom)` | 지도 초기화 | ✅ 적용 가능 |
| `addMarker(id, lat, lng, type, name)` | 마커 추가 | ✅ 적용 가능 |
| `setMarkerVisible(id, visible)` | 마커 표시/숨김 | ✅ 적용 가능 |
| `drawCircle(lat, lng, radiusKm)` | 반경 원 그리기 | ✅ 적용 가능 |
| `clearCircle()` | 반경 원 제거 | ✅ 적용 가능 |
| `showAllMarkers()` | 모든 마커 표시 | ✅ 적용 가능 |
| `setDotNetReference(ref)` | C# 콜백 설정 | ⚠️ MAUI 방식 다름 |

### ✅ **마커 스타일** (검증된 디자인)

```javascript
// 이용자: 파란 원 (20x20px)
{
    content: '<div style="width:20px;height:20px;background-color:#4285F4;border:2px solid white;border-radius:50%;box-shadow:0 2px 4px rgba(0,0,0,0.3);"></div>',
    anchor: new naver.maps.Point(10, 10)
}

// 지원사: 주황 사각형 (16x16px)
{
    content: '<div style="width:16px;height:16px;background-color:#FF9800;border:2px solid white;box-shadow:0 2px 4px rgba(0,0,0,0.3);"></div>',
    anchor: new naver.maps.Point(8, 8)
}
```

**결론**: 이 스타일을 그대로 MAUI `Resources/Raw/map.html`에 적용

---

### ✅ **Circle 스타일** (검증된 디자인)

```javascript
new naver.maps.Circle({
    map: map,
    center: new naver.maps.LatLng(lat, lng),
    radius: radiusKm * 1000, // km를 m로 변환
    fillColor: '#4285F4',
    fillOpacity: 0.2,
    strokeColor: '#4285F4',
    strokeOpacity: 0.6,
    strokeWeight: 2
});
```

**결론**: 이 스타일을 그대로 MAUI에 적용

---

## 📋 3. 필터링 로직 (Blazor Home.razor 참고)

### ✅ **반경 필터링 흐름** (검증된 UX)

```
1. 이용자 마커 클릭
   ↓
2. selectedUser 설정
   ↓
3. drawCircle(lat, lng, radius) 호출
   ↓
4. 모든 지원사에 대해 거리 계산
   ↓
5. 반경 내 → setMarkerVisible(id, true)
   반경 외 → setMarkerVisible(id, false)
   ↓
6. DataGrid 업데이트 (거리순 정렬)
```

**MAUI 적용 시 차이점**:
- Blazor: `MudDataGrid` 사용
- MAUI: `CollectionView` 사용

---

### ✅ **반경 버튼 UI** (검증된 디자인)

```razor
<MudButtonGroup Color="Color.Primary" Variant="Variant.Outlined">
    <MudButton OnClick="@(() => FilterByRadius(1))">1km</MudButton>
    <MudButton OnClick="@(() => FilterByRadius(3))">3km</MudButton>
    <MudButton OnClick="@(() => ShowAll())">전체</MudButton>
</MudButtonGroup>
```

**MAUI 적용 시**:
```xml
<HorizontalStackLayout Spacing="10">
    <Button Text="1km" Command="{Binding ChangeRadiusCommand}" CommandParameter="1.0" />
    <Button Text="3km" Command="{Binding ChangeRadiusCommand}" CommandParameter="3.0" />
    <Button Text="전체" Command="{Binding ShowAllCommand}" />
</HorizontalStackLayout>
```

---

## 📋 4. 네이버 지도 API 로드 확인 로직

### ✅ **API 로드 대기 (검증된 로직)**

```csharp
private async Task<bool> WaitForNaverMapsApi(IJSRuntime js, int timeoutMs)
{
    var startTime = DateTime.Now;
    while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
    {
        try
        {
            var isLoaded = await js.InvokeAsync<bool>("eval", 
                "typeof naver !== 'undefined' && typeof naver.maps !== 'undefined'");
            if (isLoaded) return true;
        }
        catch { }
        await Task.Delay(100);
    }
    return false;
}
```

**MAUI 적용**:
- WebView의 `Navigated` 이벤트를 사용
- `WebNavigationResult.Success` 확인 후 JS 호출

---

## 📋 5. DI 등록 패턴

### Blazor
```csharp
builder.Services.AddSingleton<MockDataService>();
```

### MAUI (현재)
```csharp
builder.Services.AddSingleton<IMockDataService, MockDataService>();
builder.Services.AddSingleton<ILocationService, LocationService>();
```

**차이점**:
- Blazor: 인터페이스 없이 직접 등록
- MAUI: 인터페이스 + 구현 패턴 (더 나은 설계)

---

## 📋 6. MAUI에 적용할 핵심 개선 사항

### ✅ **즉시 적용 가능**

1. **네이버 지도 JS 함수** (naverMap.js)
   - `initMap`, `addMarker`, `drawCircle`, `clearCircle` 그대로 사용
   - 마커 스타일 동일하게 적용

2. **필터링 로직**
   - Haversine 거리 계산 → 이미 적용됨 ✅
   - 반경 내/외 필터링 → `FilterByRadius` 메서드 활용

3. **UI 레이아웃**
   - 좌측 30% (목록) / 우측 70% (지도)
   - 반경 버튼 (1km/3km/전체)

---

### ⚠️ **MAUI에서 변경 필요**

1. **WebView 통신 방식**
   - Blazor: `DotNetObjectReference` + `JSInvokable`
   - MAUI: `EvaluateJavaScriptAsync` (단방향) 또는 Custom URL Scheme (양방향)

2. **XAML vs Razor**
   - Blazor: `@code` 블록, Razor 문법
   - MAUI: XAML + Code-behind, MVVM 패턴

3. **상태 관리**
   - Blazor: `StateHasChanged()` 호출
   - MAUI: `ObservableCollection<T>` + `INotifyPropertyChanged`

---

## 🎯 7. 다음 단계 권장 사항

### Day 5-7: BaseViewModel & MapViewModel 구현 시

**Blazor의 `Home.razor @code` 블록을 MAUI `MapViewModel`로 변환**

| Blazor Home.razor | MAUI MapViewModel |
|-------------------|-------------------|
| `private List<Person> filteredPeople` | `ObservableCollection<Person> FilteredPersons` |
| `private Person? selectedUser` | `Person? SelectedUser` (ObservableProperty) |
| `private double currentRadius` | `double CurrentRadius` (ObservableProperty) |
| `FilterByRadius(double radiusKm)` 메서드 | `ChangeRadiusCommand` (RelayCommand) |
| `ShowAll()` 메서드 | `ShowAllCommand` (RelayCommand) |
| `OnUserMarkerClicked` JS 콜백 | WebView 이벤트 핸들러 (Page 레벨) |

---

## 📚 8. 참고 파일 위치

### Blazor 프로젝트
```
MapDemo/
├── Models/Person.cs                    # 모델 (string ID)
├── Services/
│   ├── MockDataService.cs              # Mock 데이터 (검증된 좌표)
│   └── DistanceCalculator.cs           # Haversine 공식
├── Components/Pages/Home.razor         # 메인 화면 (필터링 로직)
├── wwwroot/js/naverMap.js              # 네이버 지도 JS Interop
└── Program.cs                          # DI 등록
```

### MAUI 프로젝트 (현재)
```
src/LmpLink.MAUI/
├── Models/Person.cs                    # 모델 (int ID, record type)
├── Services/
│   ├── Interfaces/
│   │   ├── IMockDataService.cs
│   │   └── ILocationService.cs
│   └── Implementation/
│       ├── MockDataService.cs          # Mock 데이터
│       └── LocationService.cs          # Haversine + 필터링
├── MauiProgram.cs                      # DI 등록 (인터페이스 패턴)
└── (ViewModels, Views - 추후 생성)
```

---

## ✅ 9. 결론

### 기존 Blazor 프로젝트에서 검증된 로직:
1. ✅ **Haversine 거리 계산** → MAUI에 이미 적용됨
2. ✅ **네이버 지도 JS 함수** → `Resources/Raw/map.html`에 적용 예정
3. ✅ **마커 스타일** (파란 원, 주황 사각형) → 그대로 적용 가능
4. ✅ **Circle 스타일** (파란 반투명 원) → 그대로 적용 가능
5. ✅ **필터링 로직** (반경 내/외 필터링) → MAUI ViewModel에 적용 예정

### MAUI에서 개선된 점:
1. ✅ **인터페이스 기반 DI** (테스트 가능, 확장 가능)
2. ✅ **record 타입 사용** (불변 데이터, `with` 표현식)
3. ✅ **MVVM 패턴** (UI와 로직 분리)
4. ✅ **크로스플랫폼** (Windows + Android)

---

**다음 작업**: Day 5-7 BaseViewModel & MapViewModel 구현 시, Blazor의 `Home.razor @code` 블록을 참고하여 MVVM 패턴으로 변환
