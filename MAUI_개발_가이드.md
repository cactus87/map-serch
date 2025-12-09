# LMP-Link MAUI 개발 가이드 (2025)

**프로젝트**: LMP-Link - Location-based Matching Platform  
**기술 스택**: .NET MAUI 9.0, C# 13, MVVM Pattern  
**플랫폼**: Windows (MSIX) + Android (APK)  
**개발 기간**: 4-6주 (1인 개발자)

---

## 📋 목차

1. [환경 설정](#1-환경-설정)
2. [프로젝트 생성](#2-프로젝트-생성)
3. [Cursor AI 규칙 활용](#3-cursor-ai-규칙-활용)
4. [개발 워크플로우](#4-개발-워크플로우)
5. [Slash Commands 활용](#5-slash-commands-활용)
6. [네이버 지도 연동](#6-네이버-지도-연동)
7. [배포 준비](#7-배포-준비)
8. [트러블슈팅](#8-트러블슈팅)

---

## 1. 환경 설정

### 1.1 필수 도구 설치

```bash
# .NET 9 SDK 설치 확인
dotnet --version
# 출력 예상: 9.0.100 이상

# MAUI Workload 설치
dotnet workload install maui

# Android SDK 설치 (Android 개발 시)
# Visual Studio Installer에서 "Mobile development with .NET" 선택

# Windows App SDK 설치 (Windows 개발 시)
# Visual Studio Installer에서 ".NET desktop development" 선택
```

### 1.2 Cursor AI 설정

1. **Cursor 재시작** (규칙 파일 로드)
   - `Ctrl + Shift + P` → `Reload Window`

2. **규칙 파일 확인**
   ```
   .cursor/rules/
   ├── project-guidelines.mdc    ✅ MAUI 프로젝트 가이드라인
   ├── 60-maui-core.mdc          ✅ MAUI 9.0 + C# 13 핵심 규칙
   └── index.mdc                 ✅ 전역 규칙
   ```

3. **Slash Commands 확인**
   ```
   .cursor/commands/
   ├── gen-maui-viewmodel.md     ✅ /maui-viewmodel
   ├── gen-maui-page.md          ✅ /maui-page
   ├── gen-maui-service.md       ✅ /maui-service
   └── gen-maui-platform.md      ✅ /maui-platform
   ```

---

## 2. 프로젝트 생성

### 2.1 MAUI 프로젝트 생성

```bash
# 프로젝트 생성 (루트 폴더에서)
dotnet new maui -n LmpLink.MAUI -o src/LmpLink.MAUI

# 디렉토리 이동
cd src/LmpLink.MAUI

# NuGet 패키지 설치
dotnet add package CommunityToolkit.Mvvm --version 8.3.2
dotnet add package Microsoft.Extensions.Logging.Debug

# 프로젝트 복원
dotnet restore
```

### 2.2 프로젝트 구조 생성

**Cursor AI에게 프롬프트 입력:**

```
📋 프롬프트:
프로젝트 루트에 다음 폴더 구조를 생성해줘:

- ViewModels/Base/
- Views/Pages/
- Views/Controls/
- Models/
- Services/Interfaces/
- Services/Implementation/
- Helpers/
- Resources/Raw/

각 폴더에 .gitkeep 파일도 생성해줘.
```

### 2.3 GlobalUsings.cs 생성

**Cursor AI에게 프롬프트 입력:**

```
📋 프롬프트:
GlobalUsings.cs 파일을 생성해줘. 다음을 포함:

- System, System.Collections.Generic, System.Linq
- Microsoft.Maui, Microsoft.Maui.Controls
- CommunityToolkit.Mvvm.ComponentModel
- CommunityToolkit.Mvvm.Input
- CommunityToolkit.Mvvm.Messaging
- LmpLink.MAUI.Models
- LmpLink.MAUI.Services
- LmpLink.MAUI.ViewModels

global using 문법 사용.
```

---

## 3. Cursor AI 규칙 활용

### 3.1 규칙 파일 작동 확인

**Cursor AI에게 질문:**

```
📋 프롬프트:
현재 프로젝트의 기술 스택과 아키텍처 패턴을 설명해줘.
```

**기대 응답:**
- .NET MAUI 9.0
- MVVM 패턴 (CommunityToolkit.Mvvm)
- C# 13 (#nullable enable, record types)
- DI Container (Microsoft.Extensions.DependencyInjection)

### 3.2 자동 코드 스타일 적용

모든 새 파일은 자동으로 다음을 준수합니다:
- ✅ `#nullable enable`
- ✅ XML 주석 (`/// <summary>`)
- ✅ Async/Await 패턴
- ✅ Early Return 패턴

---

## 4. 개발 워크플로우

### Phase 1: 기본 모델 & 서비스 (Week 1)

#### Step 1: 데이터 모델 생성

**Cursor AI 프롬프트:**

```
📋 프롬프트:
Models/Person.cs 파일을 생성해줘.

요구사항:
- record 타입 사용
- Properties: Id (int), Name (string), Type (PersonType enum), 
  Latitude (double), Longitude (double), Address (string),
  Phone (string?), Gender (string?), HasVehicle (bool)
- PersonType enum: User, Assistant

60-maui-core.mdc 규칙에 맞게 작성해줘.
```

#### Step 2: Location Service 생성

**Slash Command 사용:**

```
📋 Cursor 입력:
/maui-service LocationService
```

**추가 프롬프트 (생성 후):**

```
📋 프롬프트:
LocationService에 다음 메서드를 추가해줘:

1. CalculateDistance(lat1, lon1, lat2, lon2) - Haversine 공식
2. FilterByRadius(center, candidates, radiusKm) - 반경 내 필터링
3. SortByDistance(center, candidates) - 거리순 정렬

모든 거리는 소수점 2자리로 반올림해줘.
```

#### Step 3: Mock Data Service 생성

**Cursor AI 프롬프트:**

```
📋 프롬프트:
Services/Implementation/MockDataService.cs를 생성해줘.

요구사항:
- Interface: IMockDataService
- 메서드: GetUsersAsync(), GetAssistantsAsync(), GetAllPersonsAsync()
- 데이터: 이용자 10명 (도봉구청 중심 0-4km), 지원사 20명 (0-4km)
- 중심 좌표: 37.6688, 127.0471
- Mock ID는 고정값 사용 (1-10, 11-30)

MauiProgram.cs 등록 코드도 제공해줘.
```

---

### Phase 2: MVVM 구조 (Week 2)

#### Step 4: BaseViewModel 생성

**Cursor AI 프롬프트:**

```
📋 프롬프트:
ViewModels/Base/BaseViewModel.cs를 생성해줘.

요구사항:
- ObservableObject 상속
- Properties: IsLoading, ErrorMessage, HasError
- Methods: ExecuteAsync(operation), ExecuteAsync<T>(operation)
- Messenger 주입
- 에러 처리 래퍼 포함

60-maui-core.mdc Pattern 4 참고해서 작성해줘.
```

#### Step 5: MapViewModel 생성

**Slash Command 사용:**

```
📋 Cursor 입력:
/maui-viewmodel MapViewModel
```

**추가 프롬프트 (생성 후):**

```
📋 프롬프트:
MapViewModel에 다음 기능을 추가해줘:

Properties:
- Users (ObservableCollection<Person>)
- Assistants (ObservableCollection<Person>)
- SelectedUser (Person?)
- CurrentRadius (double, default 3.0)
- FilteredAssistants (ObservableCollection<Person>)

Commands:
- LoadDataCommand: Mock 데이터 로드
- SelectUserCommand: 이용자 선택 + 반경 필터링
- ChangeRadiusCommand: 반경 변경 (1km/3km/5km/전체)

Services:
- ILocationService, IMockDataService 주입
```

---

### Phase 3: UI & 네이버 지도 (Week 3-4)

#### Step 6: MainPage 생성

**Slash Command 사용:**

```
📋 Cursor 입력:
/maui-page MainPage
```

**추가 프롬프트 (생성 후):**

```
📋 프롬프트:
MainPage.xaml을 다음과 같이 수정해줘:

레이아웃:
- Grid 2열: 좌측 30% (목록), 우측 70% (지도)
- 좌측: CollectionView (이용자 리스트) + 반경 버튼
- 우측: WebView (네이버 지도)

반경 버튼:
- 1km, 3km, 5km, 전체 (HorizontalStackLayout)
- Command: {Binding ChangeRadiusCommand}
- CommandParameter: 1.0, 3.0, 5.0, 99.0

지도 WebView:
- Source: file:///android_asset/map.html (Android)
- Source: ms-appx-web:///Resources/Raw/map.html (Windows)
- Navigated 이벤트 핸들러: OnWebViewNavigated
```

#### Step 7: 네이버 지도 HTML 생성

**Cursor AI 프롬프트:**

```
📋 프롬프트:
Resources/Raw/map.html 파일을 생성해줘.

요구사항:
- 네이버 Maps API v3 사용
- ncpKeyId 파라미터 사용 (환경변수 NAVER_MAP_KEY)
- 초기 중심: 37.6688, 127.0471
- Zoom: 13
- JavaScript 함수:
  - window.initMap(lat, lng, zoom)
  - window.addMarker(id, type, lat, lng, name, address)
  - window.drawCircle(lat, lng, radiusKm)
  - window.clearCircle()
  - window.setMarkerVisible(id, visible)

project-guidelines.mdc의 네이버 지도 API 설정 참고해줘.
```

#### Step 8: WebView JS Interop 구현

**Cursor AI 프롬프트:**

```
📋 프롬프트:
MainPage.xaml.cs에 다음 메서드를 추가해줘:

1. InitializeMapAsync()
   - WebView.EvaluateJavaScriptAsync로 initMap 호출

2. AddMarkersAsync(List<Person> persons)
   - 각 Person마다 addMarker 호출
   - JSON 직렬화해서 전달

3. DrawRadiusCircleAsync(double lat, double lng, double radiusKm)
   - drawCircle 호출

4. FilterMarkersAsync(List<int> visibleIds)
   - setMarkerVisible 호출

ViewModel의 Property Changed 이벤트 구독해서 자동 업데이트.
```

---

### Phase 4: Platform-Specific 코드 (Week 4)

#### Step 9: Platform Service 생성

**Slash Command 사용:**

```
📋 Cursor 입력:
/maui-platform PlatformService --both
```

**추가 프롬프트 (생성 후):**

```
📋 프롬프트:
IPlatformService에 다음 메서드를 추가해줘:

- GetDeviceId(): Android는 Build.Serial, Windows는 MachineName
- RequestLocationPermissionAsync(): Android만 런타임 권한 요청
- IsConnected(): Connectivity.Current.NetworkAccess 확인
- GetPlatformName(): 플랫폼 + OS 버전

Android, Windows 구현 모두 작성해줘.
```

#### Step 10: MauiProgram.cs DI 설정

**Cursor AI 프롬프트:**

```
📋 프롬프트:
MauiProgram.cs의 CreateMauiApp 메서드를 다음과 같이 수정해줘:

Services 등록:
1. System Services:
   - IMessenger (WeakReferenceMessenger.Default)

2. Business Services:
   - IMockDataService → MockDataService
   - ILocationService → LocationService

3. Platform Services:
   - #if ANDROID → AndroidPlatformService
   - #if WINDOWS → WindowsPlatformService

4. ViewModels:
   - MapViewModel

5. Pages:
   - MainPage

6. Shell:
   - AppShell

60-maui-core.mdc Pattern 1 참고해줘.
```

---

### Phase 5: AppShell Navigation (Week 5)

#### Step 11: AppShell 설정

**Cursor AI 프롬프트:**

```
📋 프롬프트:
AppShell.xaml을 다음과 같이 수정해줘:

TabBar 구조:
1. ShellContent (지도)
   - Title: "지도"
   - Icon: "map.png"
   - Route: "main"
   - ContentTemplate: MainPage

2. ShellContent (설정)
   - Title: "설정"
   - Icon: "settings.png"
   - Route: "settings"
   - ContentTemplate: SettingsPage

FlyoutBehavior: Disabled (탭만 사용)
```

---

## 5. Slash Commands 활용

### 5.1 ViewModel 생성

```bash
# Cursor에 입력
/maui-viewmodel MatchingViewModel

# 생성 후 추가 프롬프트
Properties: SelectedUser, SelectedAssistant, MatchScore
Commands: ProposeMatchCommand, ConfirmMatchCommand
Services: IApiService 주입
```

### 5.2 Page 생성

```bash
# Cursor에 입력
/maui-page MatchingPage --with-list

# 생성 후 추가 프롬프트
좌우 2열 레이아웃:
- 좌측: 이용자 정보 (이름, 주소, 요구사항)
- 우측: 지원사 정보 (이름, 거리, 경력)
- 하단: "매칭 확정" 버튼 (Command: ConfirmMatchCommand)
```

### 5.3 Service 생성

```bash
# Cursor에 입력
/maui-service ApiService --with-httpclient

# 생성 후 추가 프롬프트
Supabase API 연동:
- BaseAddress: https://your-project.supabase.co
- Headers: apikey, Authorization
- Methods: GetPersonsAsync, CreateMatchAsync
```

### 5.4 Platform Code 생성

```bash
# Cursor에 입력
/maui-platform NotificationService --android

# 생성 후 추가 프롬프트
Android Local Notification:
- CreateNotificationChannel
- ShowNotification(title, message)
- CancelNotification(notificationId)
```

---

## 6. 네이버 지도 연동

### 6.1 API 키 설정

1. **네이버 클라우드 플랫폼 콘솔**
   - Web Dynamic Map 서비스 활성화
   - Web 서비스 URL 등록:
     - `http://localhost:5000`
     - `http://127.0.0.1:5000`

2. **User Secrets 설정**

```bash
# User Secrets 초기화
dotnet user-secrets init

# API 키 저장
dotnet user-secrets set "NaverMapApiKey" "YOUR_CLIENT_ID"
```

3. **MauiProgram.cs에서 로드**

**Cursor AI 프롬프트:**

```
📋 프롬프트:
MauiProgram.cs에서 User Secrets를 로드하는 코드를 추가해줘.

Configuration Builder:
- AddJsonFile("appsettings.json")
- AddUserSecrets<App>()
- AddEnvironmentVariables()

IConfiguration을 DI Container에 등록해줘.
```

### 6.2 WebView 디버깅

**Android:**

```bash
# Chrome DevTools로 디버그
chrome://inspect/#devices
```

**Windows:**

```bash
# Edge DevTools로 디버그 (F12)
```

---

## 7. 배포 준비

### 7.1 Android APK 빌드

```bash
# Release 빌드
dotnet publish -f net9.0-android -c Release

# APK 위치 확인
# bin/Release/net9.0-android/publish/
```

### 7.2 Windows MSIX 빌드

```bash
# Release 빌드
dotnet publish -f net9.0-windows10.0.19041.0 -c Release

# MSIX 위치 확인
# bin/Release/net9.0-windows10.0.19041.0/publish/
```

### 7.3 성능 최적화

**Cursor AI 프롬프트:**

```
📋 프롬프트:
MauiProgram.cs에 다음 최적화를 적용해줘:

1. Startup 최적화:
   - UseShellHandlers
   - SetUseLegacyRenderers(false)

2. Image 최적화:
   - Microsoft.Maui.Graphics.Skia 사용

3. CollectionView 최적화:
   - ItemsLayout CacheLength 설정

4. WebView 최적화:
   - Hardware Acceleration 활성화
```

---

## 8. 트러블슈팅

### 8.1 네이버 지도 로드 실패

**증상**: WebView에 지도가 표시되지 않음

**해결책 (Cursor AI 프롬프트):**

```
📋 프롬프트:
네이버 지도 로드 실패 문제를 디버깅하는 코드를 추가해줘:

1. WebView.Navigated 이벤트 핸들러:
   - Result 확인 (Success/Failure)
   - URL 로깅

2. JavaScript 에러 핸들러:
   - window.navermap_authFailure 정의
   - 에러 메시지 C#으로 전달

3. Network 요청 확인:
   - DevTools에서 maps.js 요청 확인
   - Referer 헤더 확인
```

### 8.2 DI 주입 실패

**증상**: `NullReferenceException` 발생

**해결책 (Cursor AI 프롬프트):**

```
📋 프롬프트:
MauiProgram.cs의 서비스 등록 순서를 검증하는 코드를 작성해줘:

1. Services 등록 체크:
   - IMockDataService 등록 확인
   - ILocationService 등록 확인

2. ViewModels 등록 체크:
   - MapViewModel 생성자 파라미터 검증

3. 순환 참조 체크:
   - 서비스 간 의존성 그래프 출력
```

### 8.3 Android 권한 거부

**증상**: Location 권한이 계속 거부됨

**해결책 (Cursor AI 프롬프트):**

```
📋 프롬프트:
Platforms/Android/AndroidManifest.xml에 다음 권한을 추가해줘:

<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />

MainActivity.cs에서 권한 요청 코드도 추가해줘:
- OnCreate에서 RequestLocationPermissionAsync 호출
```

---

## 9. 실전 개발 시나리오

### 시나리오 1: 새로운 필터 기능 추가

**요구사항**: 지원사를 "차량 보유 여부"로 필터링

**Cursor AI 프롬프트:**

```
📋 프롬프트:
MapViewModel에 차량 보유 필터를 추가해줘.

1. Property 추가:
   - FilterByVehicle (bool, default false)

2. Command 추가:
   - ToggleVehicleFilterCommand

3. 필터링 로직:
   - FilterAssistantsByRadius 메서드 수정
   - FilterByVehicle가 true면 HasVehicle==true인 지원사만 표시

4. XAML 버튼:
   - MainPage.xaml에 "차량 보유만" 토글 버튼 추가
   - Command: ToggleVehicleFilterCommand
```

### 시나리오 2: 매칭 결과 저장

**요구사항**: 매칭 확정 시 Supabase에 저장

**Cursor AI 프롬프트:**

```
📋 프롬프트:
/maui-service SupabaseService --with-httpclient

메서드 추가:
- CreateMatchAsync(userId, assistantId, matchScore)
- GetMatchHistoryAsync(userId)
- UpdateMatchStatusAsync(matchId, status)

BaseAddress: https://your-project.supabase.co
Headers:
- apikey: {SupabaseApiKey}
- Authorization: Bearer {SupabaseApiKey}

MauiProgram.cs 등록 코드도 제공해줘.
```

---

## 10. 빠른 참조

### Cursor AI 질문 예시

| 질문 | 용도 |
|------|------|
| "현재 프로젝트의 기술 스택은?" | 규칙 파일 확인 |
| "BaseViewModel 패턴 보여줘" | 60-maui-core.mdc 참조 |
| "반경 로직 어떻게 구현?" | project-guidelines.mdc 참조 |
| "네이버 지도 API 키 설정?" | project-guidelines.mdc 참조 |

### Slash Commands 요약

| Command | 용도 | 예시 |
|---------|------|------|
| `/maui-viewmodel` | ViewModel 생성 | `/maui-viewmodel MapViewModel` |
| `/maui-page` | ContentPage 생성 | `/maui-page MainPage` |
| `/maui-service` | Service 생성 | `/maui-service LocationService` |
| `/maui-platform` | Platform 코드 | `/maui-platform PlatformService --both` |

### 유용한 프롬프트 템플릿

```
📋 템플릿 1: 기능 추가
[ViewModel/Service/Page]에 [기능]을 추가해줘.

요구사항:
- [Property/Command/Method 리스트]
- [비즈니스 로직 설명]
- [UI 동작 설명]

60-maui-core.mdc 규칙에 맞게 작성해줘.
```

```
📋 템플릿 2: 버그 수정
[증상]이 발생해.

현재 코드:
[코드 붙여넣기]

예상 동작:
[설명]

실제 동작:
[설명]

디버깅 및 수정 방법 제안해줘.
```

```
📋 템플릿 3: 리팩터링
[파일명]을 다음과 같이 리팩터링해줘:

1. [변경사항 1]
2. [변경사항 2]
3. [변경사항 3]

MVVM 패턴과 DI 원칙 유지해줘.
```

---

## 11. 체크리스트

### 개발 시작 전

- [ ] .NET 9 SDK 설치됨
- [ ] MAUI Workload 설치됨
- [ ] Cursor AI 규칙 파일 로드 확인
- [ ] Slash Commands 작동 확인
- [ ] 네이버 API 키 발급됨

### 개발 중

- [ ] 모든 파일에 `#nullable enable` 있음
- [ ] ViewModel은 BaseViewModel 상속
- [ ] Services는 Interface + Implementation 분리
- [ ] MauiProgram.cs에 DI 등록됨
- [ ] XML 주석 (`/// <summary>`) 작성됨

### 배포 전

- [ ] Android APK 빌드 성공
- [ ] Windows MSIX 빌드 성공
- [ ] 네이버 지도 로드 확인
- [ ] 반경 필터 동작 확인
- [ ] 성능 테스트 (앱 시작 <2초, 지도 로드 <3초)

---

## 12. 추가 리소스

### 공식 문서

- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [Naver Maps API](https://navermaps.github.io/maps.js.ncp/)

### 프로젝트 문서

- `PRD.md` - 요구사항 정의서
- `개발_로드맵.md` - 상세 개발 일정
- `디자인_시스템.md` - UI/UX 가이드라인

---

**작성일**: 2025-12-09  
**버전**: 1.0  
**대상**: 1인 개발자 (Cursor AI 활용)  
**예상 기간**: 4-6주
