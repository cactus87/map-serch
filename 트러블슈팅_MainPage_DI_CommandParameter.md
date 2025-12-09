# 🔧 트러블슈팅: MainPage 즉시 종료 문제 해결

**날짜**: 2025-12-10  
**소요 시간**: 약 3시간  
**난이도**: ⭐⭐⭐⭐ (High)  

---

## 📋 목차
1. [문제 증상](#문제-증상)
2. [진단 과정](#진단-과정)
3. [근본 원인](#근본-원인)
4. [해결 방법](#해결-방법)
5. [학습 내용](#학습-내용)
6. [예방 가이드라인](#예방-가이드라인)

---

## 문제 증상

### 관찰된 현상
```powershell
PS> dotnet run --framework net10.0-windows10.0.19041.0
# 앱이 즉시 종료 (2-3초 이내)
# Exit code: 3221226107 또는 정상 종료 코드
```

### 환경
- **.NET SDK**: 10.0.100
- **MAUI**: 10.0.0
- **OS**: Windows 11 (10.0.22631)
- **WebView2 Runtime**: 143.0.3650.66 (설치됨)

### 특이사항
- ✅ `TestPage` (간단한 UI): **정상 실행**
- ❌ `MainPage` (WebView + MVVM): **즉시 종료**
- 빌드: 0 errors, 0 warnings
- Windows Event Viewer: MAUI 관련 에러 없음

---

## 진단 과정

### 1단계: 격리 테스트 (Isolation)
```csharp
// TestPage.xaml.cs (매개변수 없는 생성자)
public TestPage()
{
    InitializeComponent();  // ✅ 성공
}
```

**결과**: TestPage 성공 → MAUI 프레임워크 자체는 정상  
**결론**: MainPage의 특정 로직이 문제

---

### 2단계: DI 주입 시도 (실패)

#### 시도 A: Shell DataTemplate + 생성자 주입
```csharp
// MainPage.xaml.cs
public MainPage(MapViewModel viewModel, IMapService mapService)
{
    InitializeComponent();
    _viewModel = viewModel;
    _mapService = mapService;
}
```

**문제**: Shell의 `DataTemplate`은 **기본 생성자만 호출 가능**  
**결과**: DI 서비스 주입 실패 → 앱 종료

#### 시도 B: App.xaml.cs에서 IServiceProvider 주입
```csharp
public App(IServiceProvider serviceProvider)  // ❌ 컴파일 성공, 런타임 실패
{
    _serviceProvider = serviceProvider;
    InitializeComponent();
}
```

**문제**: `MauiApp.CreateBuilder()` → `builder.Build()` 과정에서 `App` 생성 시 매개변수 없는 생성자 호출  
**결과**: 앱이 생성조차 안 됨

---

### 3단계: 디버그 로깅 추가 ⭐ (핵심 돌파구)

#### 로깅 인프라 구축
```csharp
// MauiProgram.cs
private static readonly string LogPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
    "maui_debug.log");

public static void Log(string message)
{
    var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
    File.AppendAllText(LogPath, line + Environment.NewLine);
    System.Diagnostics.Debug.WriteLine(line);
}
```

#### 생명주기 각 단계 로깅
```csharp
// MauiProgram.cs
Log("=== MauiProgram.CreateMauiApp START ===");
Log("[1] MauiAppBuilder created");
Log("[2] UseMauiApp<App> configured");
// ...

// App.xaml.cs
Log("=== App Constructor START ===");
Log("=== App.CreateWindow START ===");
// ...

// MainPage.xaml.cs
Log("=== MainPage Constructor START ===");
Log("[1] Calling InitializeComponent...");
Log("[2] InitializeComponent done");
Log("[3] Getting services...");
// ... (각 단계마다 로깅)
```

#### 로그 결과 (Desktop/maui_debug.log)
```
[01:07:42.448] === MainPage Constructor START ===
[01:07:42.448] [1] Calling InitializeComponent...
[01:07:42.480] [2] InitializeComponent done
[01:07:42.480] [3] Getting services...
[01:07:42.480] [4] Services obtained
[01:07:42.480] [5] Getting MapViewModel...
[01:07:42.481] [6] MapViewModel obtained
[01:07:42.482] [7] Getting IMapService...
[01:07:42.482] [8] IMapService obtained
[01:07:42.485] !!! MainPage Constructor FAILED: 
Parameter "parameter" (object) cannot be of type System.String, 
as the command type requires an argument of type System.Double. 
(Parameter 'parameter')
   at CommunityToolkit.Mvvm.Input.RelayCommand`1.ThrowArgumentExceptionForInvalidCommandArgument(Object parameter)
   at CommunityToolkit.Mvvm.Input.RelayCommand`1.CanExecute(Object parameter)
   at Microsoft.Maui.Controls.CommandElement.GetCanExecute(ICommandElement commandElement)
   at Microsoft.Maui.Controls.Button.get_IsEnabledCore()
   ...
```

**발견**: `BindingContext = _viewModel;` 설정 후 Command 바인딩 시 **타입 불일치 에러**

---

## 근본 원인

### 원인 1: XAML CommandParameter 타입 불일치

#### 문제 코드
```xml
<!-- MainPage.xaml -->
<Button 
    Command="{Binding ChangeRadiusCommand}"
    CommandParameter="1.0" />  <!-- ❌ String 타입 -->
```

```csharp
// MapViewModel.cs
[RelayCommand]
private void ChangeRadius(double radius)  // ✅ double 타입 기대
{
    CurrentRadius = radius;
}

// CommunityToolkit.Mvvm이 자동 생성:
public RelayCommand<double> ChangeRadiusCommand { get; }
```

#### 타입 검증 흐름
```
1. BindingContext = viewModel 설정
   ↓
2. XAML 바인딩 평가 시작
   ↓
3. Command="{Binding ChangeRadiusCommand}" 바인딩
   ↓
4. CommandParameter="1.0" 평가 → System.String
   ↓
5. RelayCommand<double>.CanExecute(object parameter) 호출
   ↓
6. 타입 검증: parameter is string ≠ double
   ↓
7. ThrowArgumentExceptionForInvalidCommandArgument()
   ↓
8. 앱 크래시
```

---

### 원인 2: MAUI DI 패턴 미숙지

#### MAUI의 DI 제약사항
- `Shell.DataTemplate`은 **기본 생성자(매개변수 없음)** 필수
- `App` 클래스는 **생성자 주입 불가**
- Page 생성 시점에 `IServiceProvider` 직접 접근 필요

#### 올바른 패턴
```csharp
public MainPage()  // ✅ 기본 생성자
{
    InitializeComponent();
    
    // IPlatformApplication.Current.Services를 통해 DI 컨테이너 접근
    var services = IPlatformApplication.Current?.Services;
    if (services == null) return;
    
    _viewModel = services.GetRequiredService<MapViewModel>();
    _mapService = services.GetRequiredService<IMapService>();
    BindingContext = _viewModel;
}
```

---

## 해결 방법

### 해결책 1: XAML에서 타입 명시

#### Before (잘못된 코드)
```xml
<Button CommandParameter="1.0" />  <!-- String -->
<Button CommandParameter="3.0" />  <!-- String -->
<Button CommandParameter="5.0" />  <!-- String -->
```

#### After (수정된 코드)
```xml
<Button Command="{Binding ChangeRadiusCommand}">
    <Button.CommandParameter>
        <x:Double>1.0</x:Double>  <!-- ✅ System.Double -->
    </Button.CommandParameter>
</Button>

<Button Command="{Binding ChangeRadiusCommand}">
    <Button.CommandParameter>
        <x:Double>3.0</x:Double>
    </Button.CommandParameter>
</Button>

<Button Command="{Binding ChangeRadiusCommand}">
    <Button.CommandParameter>
        <x:Double>5.0</x:Double>
    </Button.CommandParameter>
</Button>
```

---

### 해결책 2: DI 서비스 접근 방식 변경

#### Before (실패한 시도)
```csharp
public MainPage(MapViewModel viewModel, IMapService mapService)  // ❌
{
    InitializeComponent();
    _viewModel = viewModel;
    _mapService = mapService;
}
```

#### After (성공)
```csharp
public partial class MainPage : ContentPage
{
    private MapViewModel? _viewModel;
    private IMapService? _mapService;

    public MainPage()  // ✅ 기본 생성자
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services;
        if (services == null)
        {
            MauiProgram.Log("Services not available");
            return;
        }

        _viewModel = services.GetRequiredService<MapViewModel>();
        _mapService = services.GetRequiredService<IMapService>();
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        MapWebView.Navigated += OnWebViewNavigated;
    }
}
```

---

### 해결책 3: Null-safe 패턴 적용

#### 필드를 Nullable로 선언
```csharp
private MapViewModel? _viewModel;  // nullable
private IMapService? _mapService;  // nullable
```

#### 모든 메서드에서 Null 체크
```csharp
protected override async void OnAppearing()
{
    base.OnAppearing();
    
    if (_viewModel == null) return;  // ✅ Early return
    
    if (_viewModel.Users.Count == 0)
    {
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}

private async Task HandleRadiusChanged()
{
    if (_viewModel == null || _mapService == null) return;  // ✅ Null 체크
    
    if (_viewModel.SelectedUser != null)
    {
        var user = _viewModel.SelectedUser;
        await _mapService.DrawCircleAsync(user.Latitude, user.Longitude, _viewModel.CurrentRadius);
    }
}
```

---

## 학습 내용

### 1. XAML 타입 시스템

#### 기본 타입 사용법
```xml
<!-- 숫자 타입 -->
<Button.CommandParameter>
    <x:Double>3.14159</x:Double>
</Button.CommandParameter>

<Label.FontSize>
    <x:Int32>16</x:Int32>
</Label.FontSize>

<!-- Boolean -->
<Switch.IsToggled>
    <x:Boolean>True</x:Boolean>
</Switch.IsToggled>

<!-- String (따옴표 사용 가능) -->
<Label Text="Hello" />
```

#### 타입 변환 우선순위
1. **명시적 타입**: `<x:Double>1.0</x:Double>` → 항상 double
2. **암묵적 변환**: `CommandParameter="1.0"` → String (XAML 파서의 기본 동작)
3. **TypeConverter**: 일부 속성은 자동 변환 (예: Color, Thickness)

---

### 2. CommunityToolkit.Mvvm RelayCommand<T> 동작

#### Source Generator가 생성하는 코드
```csharp
// 개발자가 작성한 코드:
[RelayCommand]
private void ChangeRadius(double radius) { }

// 자동 생성되는 코드:
public RelayCommand<double> ChangeRadiusCommand { get; private set; }

public ChangeRadiusCommand()
{
    ChangeRadiusCommand = new RelayCommand<double>(
        execute: (radius) => ChangeRadius(radius),
        canExecute: (radius) => 
        {
            // ⚠️ 여기서 타입 검증 발생
            if (radius is not double)
                throw new ArgumentException(...);
            return true;
        }
    );
}
```

#### 바인딩 시점 검증
- **BindingContext 설정 시**: Command 바인딩 평가
- **CanExecute 호출 시**: 타입 검증 실행
- **타입 불일치 시**: 즉시 Exception 발생 → 앱 크래시

---

### 3. MAUI DI 생명주기

#### 정상 초기화 흐름
```
[앱 시작]
    ↓
MauiProgram.CreateMauiApp()
    ↓ (DI 컨테이너 빌드)
builder.Services.AddSingleton<IMockDataService, MockDataService>();
builder.Services.AddTransient<MapViewModel>();
builder.Services.AddTransient<MainPage>();
    ↓
var app = builder.Build();  // MauiApp 인스턴스 생성
    ↓
[플랫폼별 진입점 실행]
    ↓
App() 생성자  // ← App 클래스는 매개변수 없는 생성자만 허용
    ↓
App.CreateWindow(IActivationState activationState)
    ↓
new AppShell()
    ↓
Shell.DataTemplate 평가 → MainPage 생성
    ↓
MainPage()  // ← 기본 생성자만 호출됨
    ↓ (IPlatformApplication.Current.Services로 DI 접근)
services.GetRequiredService<MapViewModel>()
    ↓
InitializeComponent()
    ↓
BindingContext = viewModel;  // ← Command 바인딩 검증 시점
    ↓
OnAppearing()
```

#### DI 컨테이너 접근 방법
| 방법 | 가능 여부 | 이유 |
|------|-----------|------|
| 생성자 주입 | ❌ | Shell DataTemplate은 기본 생성자만 호출 |
| `App(IServiceProvider)` | ❌ | App 생성자는 매개변수 불가 |
| `IPlatformApplication.Current.Services` | ✅ | 정적 접근자로 DI 컨테이너 획득 |

---

### 4. 디버깅 전략

#### 파일 로깅의 중요성
```csharp
// 콘솔 출력만 사용 시 문제:
System.Diagnostics.Debug.WriteLine("Test");  
// → dotnet run에서 보이지 않음 (Visual Studio 디버거 필요)

// 파일 로그 사용:
File.AppendAllText(logPath, message);
// → 앱 종료 후에도 로그 확인 가능
```

#### 로깅 포인트 선정
1. **생명주기 진입점**: `MauiProgram.CreateMauiApp()`, `App()`, `CreateWindow()`
2. **페이지 생성**: `MainPage()` 생성자의 각 단계
3. **바인딩 전후**: `BindingContext` 설정 전/후
4. **에러 핸들러**: `try-catch`로 스택 트레이스 캡처

---

## 예방 가이드라인

### ✅ XAML CommandParameter 작성 규칙

#### 규칙 1: 타입 명시 필수
```xml
<!-- ❌ 잘못된 예 -->
<Button CommandParameter="123" />      <!-- String -->
<Button CommandParameter="true" />     <!-- String -->
<Button CommandParameter="1.5" />      <!-- String -->

<!-- ✅ 올바른 예 -->
<Button.CommandParameter>
    <x:Int32>123</x:Int32>
</Button.CommandParameter>

<Button.CommandParameter>
    <x:Boolean>True</x:Boolean>
</Button.CommandParameter>

<Button.CommandParameter>
    <x:Double>1.5</x:Double>
</Button.CommandParameter>
```

#### 규칙 2: ViewModel과 타입 일치
```csharp
// ViewModel
[RelayCommand]
private void DoSomething(int value) { }  // int 타입

// XAML (일치해야 함)
<Button.CommandParameter>
    <x:Int32>42</x:Int32>  <!-- ✅ int -->
</Button.CommandParameter>
```

---

### ✅ MAUI DI 패턴

#### 규칙 1: Page는 항상 기본 생성자
```csharp
// ✅ 올바른 패턴
public partial class MainPage : ContentPage
{
    public MainPage()  // 매개변수 없음
    {
        InitializeComponent();
        
        var services = IPlatformApplication.Current?.Services;
        // DI에서 서비스 가져오기
    }
}
```

#### 규칙 2: Null 체크 필수
```csharp
var services = IPlatformApplication.Current?.Services;
if (services == null)
{
    // 로깅 또는 fallback 처리
    return;
}

_viewModel = services.GetRequiredService<MapViewModel>();
```

#### 규칙 3: Nullable 필드 사용
```csharp
private MapViewModel? _viewModel;  // nullable
private IMapService? _mapService;  // nullable

// 모든 메서드에서 null 체크
private void SomeMethod()
{
    if (_viewModel == null || _mapService == null) return;
    
    // 로직 실행
}
```

---

### ✅ 디버깅 체크리스트

#### 앱 즉시 종료 시 확인사항
1. **파일 로그 추가** (Desktop에 저장)
   ```csharp
   File.AppendAllText(logPath, message);
   ```

2. **생명주기 로깅**
   - MauiProgram.CreateMauiApp
   - App 생성자
   - CreateWindow
   - Page 생성자 (각 단계)

3. **타입 검증**
   - XAML CommandParameter 타입 확인
   - ViewModel RelayCommand<T> 제네릭 타입 확인

4. **DI 서비스 확인**
   - `MauiProgram.cs`에 서비스 등록 여부
   - Page에서 `IPlatformApplication.Current.Services` 사용 여부

---

### ✅ 코드 리뷰 체크리스트

#### XAML 검토
- [ ] CommandParameter에 타입 명시 (`<x:Double>`, `<x:Int32>`)
- [ ] Binding 경로 오타 확인
- [ ] StaticResource 키 존재 여부

#### C# 검토
- [ ] Page 생성자가 기본 생성자인가?
- [ ] DI 서비스 접근에 `IPlatformApplication.Current.Services` 사용하는가?
- [ ] Nullable 필드에 null 체크가 있는가?
- [ ] RelayCommand<T>의 T와 CommandParameter 타입이 일치하는가?

#### 디버깅
- [ ] 파일 로그가 추가되어 있는가?
- [ ] Try-catch로 스택 트레이스 캡처하는가?

---

## 참고 자료

### 공식 문서
- [MAUI Dependency Injection](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/dependency-injection)
- [CommunityToolkit.Mvvm RelayCommand](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand)
- [XAML Type System](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/essential-xaml-syntax)

### Git Commit
- **Commit**: `2144a9f` - fix: resolve MainPage crash with proper DI and CommandParameter types
- **Branch**: main
- **날짜**: 2025-12-10

---

## 요약

| 항목 | 내용 |
|------|------|
| **근본 원인** | XAML CommandParameter 타입 불일치 (String vs Double) |
| **핵심 해결** | `<x:Double>1.0</x:Double>` 명시적 타입 지정 |
| **학습 포인트** | MAUI DI 패턴, CommunityToolkit.Mvvm 타입 검증 |
| **디버깅 키** | 파일 로깅 + 생명주기 단계별 추적 |
| **소요 시간** | 약 3시간 (격리 테스트 → DI 시도 → 로깅 → 해결) |

---

**작성자**: AI Assistant  
**검토자**: 사용자  
**버전**: 1.0  
**상태**: ✅ 해결 완료
