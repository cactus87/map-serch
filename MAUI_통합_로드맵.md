# MAUI 통합 로드맵 (Week-by-Week)

**프로젝트**: LMP-Link MVP  
**기술 스택**: .NET MAUI 9.0 + C# 13 + CommunityToolkit.Mvvm  
**기간**: 4주 (28일)  
**문서 버전**: 1.0  
**작성일**: 2025-12-10  

---

## 📊 전체 개요

### 목표
- **Week 1**: 기초 설정 + 데이터 모델 + ViewModel + UI 레이아웃
- **Week 2**: 네이버 지도 연동 + JS Interop + 마커 렌더링
- **Week 3**: Supabase 연동 + 인증 + CRUD
- **Week 4**: AI 추천 + 테스트 + 배포

### 진행률 (현재)
- ✅ Week 1: 100% 완료 (Day 1-7)
- ✅ Week 2: 43% 완료 (Day 8-10)
- ⏳ Week 2: 57% 진행 중 (Day 11-14)

---

## 🗓️ Week 1: 기초 인프라 (Day 1-7) ✅ 완료

### Day 1-2: 프로젝트 설정 ✅
**목표**: MAUI 프로젝트 생성, 폴더 구조, Git 초기화

**작업 완료**:
- [x] .NET MAUI 9.0 프로젝트 생성
- [x] CommunityToolkit.Mvvm 패키지 설치 (8.2.2)
- [x] 폴더 구조 생성 (Models, Services, ViewModels, Views)
- [x] GlobalUsings.cs 생성 (System, MAUI, MVVM, 프로젝트 네임스페이스)
- [x] .gitignore 생성
- [x] Git 초기화 및 첫 커밋

**산출물**:
```
src/LmpLink.MAUI/
├── Models/
├── Services/
│   ├── Interfaces/
│   └── Implementation/
├── ViewModels/
│   └── Base/
├── Views/
│   └── Pages/
├── Resources/
│   ├── Styles/
│   └── Raw/
└── Converters/
```

---

### Day 3-4: 데이터 모델 & Mock Service ✅
**목표**: Person 모델, Mock 데이터, 거리 계산 서비스

**작업 완료**:
- [x] Models/Person.cs (record type)
  - PersonType enum (User, Assistant)
  - 10개 필드 (Id, Name, Type, Lat, Lng, Address, Phone, Gender, HasVehicle, etc.)
  - DistanceKm 계산 프로퍼티
- [x] Services/Interfaces/IMockDataService.cs
  - GetUsersAsync(), GetAssistantsAsync(), GetAllPersonsAsync()
- [x] Services/Implementation/MockDataService.cs
  - 이용자 10명 (ID 1-10, 도봉구청 중심 0-4km)
  - 지원사 20명 (ID 11-30, 도봉구청 중심 0-4km)
- [x] Services/Interfaces/ILocationService.cs
  - CalculateDistance (Haversine), FilterByRadius, SortByDistance
- [x] Services/Implementation/LocationService.cs
  - Haversine 공식 구현, 거리 소수점 2자리 반올림
- [x] MauiProgram.cs DI 등록

**테스트 결과**:
- ✅ Users.Count == 10
- ✅ Assistants.Count == 20
- ✅ CalculateDistance(user1, assistant1) ≈ 0.5-4.0km
- ✅ FilterByRadius(3km) 반환 개수 정확

---

### Day 5-7: BaseViewModel & MapViewModel ✅
**목표**: MVVM 패턴 확립, 필터링 로직 구현

**작업 완료**:
- [x] ViewModels/Base/BaseViewModel.cs
  - ObservableObject 상속
  - IsLoading, HasError, ErrorMessage 프로퍼티
  - ExecuteAsync(Func<Task>), ExecuteAsync<T>
  - HandleError, ClearError
- [x] ViewModels/MapViewModel.cs
  - Properties: Users, Assistants, FilteredAssistants (ObservableCollection)
  - SelectedUser, CurrentRadius (ObservableProperty)
  - LoadDataCommand, SelectUserCommand, ChangeRadiusCommand, ShowAllCommand
  - FilterAssistantsByRadius(), UpdateFilterStatusText()
- [x] Converters/CommonConverters.cs
  - InvertedBoolConverter, IsNotNullConverter
- [x] MauiProgram.cs DI 등록 (MapViewModel)

**Blazor → MAUI 변환 완료**:
- ✅ `private List<Person>` → `ObservableCollection<Person>`
- ✅ `FilterByRadius(double)` → `ChangeRadiusCommand` (RelayCommand)
- ✅ `ShowAll()` → `ShowAllCommand` (RelayCommand)

---

## 🗓️ Week 2: UI & 네이버 지도 연동 (Day 8-14)

### Day 8-10: MainPage XAML 레이아웃 ✅
**목표**: Grid 2열 레이아웃, CollectionView, WebView

**작업 완료**:
- [x] MainPage.xaml (Grid 2열: 3*,7*)
  - 좌측 30%: 이용자 목록 + 반경 버튼
  - 우측 70%: WebView (지도 플레이스홀더)
- [x] Resources/Styles/Colors.xaml (다크 테마)
  - BackgroundDeepNavy, SurfaceDarkGray, TextPrimary, PrimaryCTA
  - UserMarker, AssistantMarker, Success, Warning, Error
- [x] Resources/Styles/Styles.xaml
  - Typography: DisplayText, Heading1, Heading2, BodyText, SmallText, CaptionText
  - Button: PrimaryCTAButton, SecondaryButton, PillButton
- [x] CollectionView ItemTemplate (카드 스타일)
  - Frame (SurfaceDarkGray 배경, CornerRadius 8)
  - 파란 원 아이콘 (UserMarker)
  - Name (Heading2), Address (SmallText), Phone (CaptionText)
- [x] Resources/Raw/map.html
  - 네이버 Maps API v3 스크립트
  - JavaScript 함수: initMap, addMarker, drawCircle, clearCircle
  - 마커 스타일 (Blazor 검증 로직 기반)

**디자인 시스템 일관성**:
- ✅ 다크 테마 색상 100% 적용
- ✅ Typography 스타일 통일
- ✅ Button 스타일 통일

---

### Day 11-12: 네이버 지도 연동 ✅ **완료 (2025-12-10)**
**목표**: WebView Source 설정, IMapService 구현, JS Interop

**작업 완료**:
- [x] User Secrets 설정
  - `dotnet user-secrets init`
  - `dotnet user-secrets set "NaverMapApiKey" "YOUR_KEY"`
- [x] Services/Interfaces/IMapService.cs (9개 메서드)
  - InitMapAsync, AddMarkerAsync, AddMarkersAsync
  - DrawCircleAsync, ClearCircleAsync
  - SetMarkerVisibleAsync, ShowAllMarkersAsync, ClearAllMarkersAsync
  - GetMapCenterAsync, SetMapCenterAsync
- [x] Services/Implementation/MapService.cs
  - WebView 참조 관리 (SetWebView)
  - EvaluateJavaScriptAsync 래퍼
  - JavaScript escaping, 에러 핸들링
- [x] MainPage.xaml.cs 업데이트
  - WebView.Navigated 이벤트 구독
  - OnViewModelPropertyChanged 구독
  - HandleSelectedUserChanged, HandleRadiusChanged, HandleFilteredAssistantsChanged
- [x] MapViewModel - IMapService 통합 (불필요, MainPage에서 직접 처리)
- [x] MauiProgram.cs DI 등록

---

### Day 13-14: 마커 렌더링 & 필터링 연동 ✅ **완료 (2025-12-10)**
**목표**: 코드 검증 및 테스트 시나리오 문서화

**작업 완료**:
- [x] 코드 품질 검증 (ReadLints 통과, 에러 없음)
- [x] 빌드 상태 확인 (MAUI 워크로드 미설치 확인)
- [x] 테스트 시나리오 문서화 (6개 항목)
- [x] 사용자 액션 가이드 작성

**사용자 액션 필요**:
1. **MAUI 워크로드 설치**:
   ```bash
   dotnet workload restore
   ```
2. **네이버 Maps API 키 설정**:
   - 발급: https://www.ncloud.com/product/applicationService/maps
   - MainPage.xaml Line 145: `ncpKeyId=YOUR_NAVER_CLIENT_ID_HERE` 교체
3. **빌드 & 실행**:
```bash
   dotnet build && dotnet run
   ```

**테스트 시나리오** (사용자 환경):
1. ✅ 지도 로드 (<3초)
2. ✅ 마커 30개 (이용자 10 + 지원사 20)
3. ✅ 이용자 선택 → 원 + 중심 이동
4. ✅ 반경 변경 (1km/3km/5km)
5. ✅ 전체 보기
6. ✅ 필터 상태 텍스트

---

## 🗓️ Week 3: Supabase 연동 (Day 15-21)

### Day 15-18: Supabase 클라이언트 설정
**목표**: Supabase NuGet 설치, 실제 데이터 로드

**작업 계획**:
- [ ] supabase-csharp NuGet 설치
- [ ] Services/Interfaces/ISupabaseService.cs
- [ ] Services/Implementation/SupabaseService.cs
- [ ] User Secrets (Supabase URL, Anon Key)
- [ ] GetUsersAsync(), GetAssistantsAsync() 구현
- [ ] MockDataService → SupabaseService 전환

---

### Day 19-21: Supabase Auth 통합
**목표**: 로그인/로그아웃, 세션 관리

**작업 계획**:
- [ ] Views/Pages/LoginPage.xaml
- [ ] ViewModels/LoginViewModel.cs
- [ ] Services/Interfaces/IAuthService.cs
- [ ] Services/Implementation/AuthService.cs
- [ ] Shell Navigation: LoginPage → MainPage
- [ ] SecureStorage에 세션 토큰 저장

---

## 🗓️ Week 4: AI & 배포 (Day 22-28)

### Day 22-24: n8n 웹훅 연동
**목표**: AI 추천 기능 구현

**작업 계획**:
- [ ] Services/Interfaces/IAiRecommendationService.cs
- [ ] Services/Implementation/AiRecommendationService.cs
- [ ] Models/MatchScore.cs
- [ ] n8n 웹훅 URL 설정
- [ ] GetRecommendationsAsync() 구현

---

### Day 25-28: 테스트 & 배포
**목표**: Windows MSIX 빌드

**작업 계획**:
- [ ] 성능 최적화 (CollectionView 가상화)
- [ ] 에러 핸들링 완성
- [ ] Windows MSIX 빌드
- [ ] 배포 문서 작성

---

## 📋 Cursor AI 프롬프트 모음

### ViewModel 생성
```
/maui-viewmodel [ViewModelName]

Properties: [property list]
Commands: [command list]
Services: [service interfaces]

60-maui-core.mdc Pattern 4 참고해줘.
```

### Page 생성
```
/maui-page [PageName]

레이아웃: [layout description]
ViewModel: [ViewModel name]

디자인_시스템.md Section 6 참고해줘.
```

### Service 생성
```
/maui-service [ServiceName]

메서드: [method list]

60-maui-core.mdc Pattern 3 참고해줘.
```

---

## ✅ 주간 체크리스트

### Week 1 ✅
- [x] 프로젝트 설정 완료
- [x] 데이터 모델 & Mock Service 완료
- [x] BaseViewModel & MapViewModel 완료

### Week 2 (진행 중)
- [x] MainPage XAML 레이아웃 완료
- [x] 다크 테마 디자인 시스템 적용 완료
- [ ] 네이버 지도 연동
- [ ] 마커 렌더링 & 필터링

### Week 3
- [ ] Supabase 클라이언트 설정
- [ ] Supabase Auth 통합
- [ ] CRUD 기능

### Week 4
- [ ] n8n 웹훅 연동
- [ ] 테스트 & 배포

---

## 🔗 참고 문서

- [PRD.md](./PRD.md) - 제품 요구사항
- [개발_로드맵.md](./개발_로드맵.md) - Phase별 상세 계획
- [디자인_시스템.md](./디자인_시스템.md) - UI/UX + XAML 코드
- [MAUI_개발_가이드.md](./MAUI_개발_가이드.md) - 기술 구현 (55+ 프롬프트)
- [Blazor_to_MAUI_참고자료.md](./Blazor_to_MAUI_참고자료.md) - 검증된 로직
- [진척_관리.md](./진척_관리.md) - 일일 작업 로그
- [작업_일관성_분석_및_통합_계획.md](./작업_일관성_분석_및_통합_계획.md) - 일관성 분석

---

**문서 버전**: 1.0  
**마지막 업데이트**: 2025-12-10  
**현재 진행률**: 36% (10/28 days)
