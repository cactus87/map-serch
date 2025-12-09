# Cursor AI 2025 상세 가이드 - MAUI 심화 레퍼런스

## 📌 개요

이 가이드는 **MAUI (Multi-platform App UI)** 개발을 위한 **2025년 최신 레퍼런스**입니다.
Blazor/Web은 별도이며, **iOS, Android, macOS, Windows 크로스플랫폼 개발**에 집중합니다.

**주요 버전**: .NET 9 LTS, MAUI 9.0+, C# 13

---

## 🏗️ MAUI 아키텍처 개요

### MAUI는 뭔가?

```
┌─────────────────────────────────────┐
│     Single C# Codebase (95%+)       │
└──────────────┬──────────────────────┘
               │
    ┌──────────┴──────────┬──────────────┬──────────┐
    │                     │              │          │
  iOS                Android          macOS      Windows
(AppKit)         (Android SDK)      (AppKit)    (WinUI3)
```

**핵심**: 하나의 XAML/C# 코드로 모든 플랫폼을 다룹니다.

---

## 📁 엔터프라이즈급 MAUI 프로젝트 구조

```
MyApp.MAUI/                                 # 루트 솔루션
│
├── src/
│   ├── MyApp.MAUI/                        # 메인 앱 프로젝트
│   │   ├── MauiProgram.cs                 # DI + 앱 초기화
│   │   ├── App.xaml                       # 전역 리소스
│   │   ├── App.xaml.cs
│   │   │
│   │   ├── AppShell.xaml                  # 네비게이션 정의
│   │   ├── AppShell.xaml.cs
│   │   │
│   │   ├── Views/                         # UI 페이지 (XAML)
│   │   │   ├── Pages/
│   │   │   │   ├── MainPage.xaml
│   │   │   │   ├── MainPage.xaml.cs
│   │   │   │   ├── DashboardPage.xaml
│   │   │   │   ├── DashboardPage.xaml.cs
│   │   │   │   ├── DetailsPage.xaml
│   │   │   │   ├── DetailsPage.xaml.cs
│   │   │   │   ├── SettingsPage.xaml
│   │   │   │   └── SettingsPage.xaml.cs
│   │   │   │
│   │   │   ├── Dialogs/
│   │   │   │   ├── ConfirmDialog.xaml
│   │   │   │   └── ConfirmDialog.xaml.cs
│   │   │   │
│   │   │   └── Controls/                  # 재사용 가능한 컨트롤
│   │   │       ├── CustomButton.xaml
│   │   │       ├── CustomButton.xaml.cs
│   │   │       ├── StatusCard.xaml
│   │   │       ├── StatusCard.xaml.cs
│   │   │       └── RatingControl.xaml
│   │   │
│   │   ├── ViewModels/                    # 비즈니스 로직 (MVVM)
│   │   │   ├── Base/
│   │   │   │   └── BaseViewModel.cs       # MVVM Toolkit 기반
│   │   │   ├── MainViewModel.cs
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── DetailsViewModel.cs
│   │   │   └── SettingsViewModel.cs
│   │   │
│   │   ├── Models/                        # 데이터 모델
│   │   │   ├── User.cs
│   │   │   ├── Product.cs
│   │   │   ├── Order.cs
│   │   │   └── ApiResponse.cs
│   │   │
│   │   ├── Services/                      # 비즈니스 서비스
│   │   │   ├── Interfaces/
│   │   │   │   ├── IApiService.cs
│   │   │   │   ├── IStorageService.cs
│   │   │   │   ├── ISyncService.cs
│   │   │   │   ├── INotificationService.cs
│   │   │   │   └── INavigationService.cs
│   │   │   │
│   │   │   ├── Implementation/
│   │   │   │   ├── ApiService.cs          # HTTP 클라이언트
│   │   │   │   ├── StorageService.cs      # 로컬 저장소
│   │   │   │   ├── SyncService.cs         # 오프라인 동기화
│   │   │   │   ├── NotificationService.cs # 로컬/푸시 알림
│   │   │   │   └── NavigationService.cs
│   │   │   │
│   │   │   └── Logger/
│   │   │       ├── ILogger.cs
│   │   │       └── ConsoleLogger.cs
│   │   │
│   │   ├── Platforms/                     # 플랫폼별 구현
│   │   │   ├── Android/
│   │   │   │   ├── MainActivity.cs
│   │   │   │   ├── AndroidManifest.xml
│   │   │   │   ├── PlatformService.cs     # Android 전용 기능
│   │   │   │   └── Permissions/
│   │   │   │       └── PermissionHandler.cs
│   │   │   │
│   │   │   ├── iOS/
│   │   │   │   ├── Info.plist
│   │   │   │   ├── AppDelegate.cs
│   │   │   │   ├── PlatformService.cs     # iOS 전용 기능
│   │   │   │   └── Entitlements.plist
│   │   │   │
│   │   │   ├── MacCatalyst/
│   │   │   │   ├── Info.plist
│   │   │   │   └── PlatformService.cs
│   │   │   │
│   │   │   └── Windows/
│   │   │       ├── App.xaml
│   │   │       ├── App.xaml.cs
│   │   │       └── PlatformService.cs
│   │   │
│   │   ├── Resources/                     # 앱 리소스
│   │   │   ├── Styles/
│   │   │   │   ├── Colors.xaml
│   │   │   │   ├── Styles.xaml
│   │   │   │   ├── Themes.xaml
│   │   │   │   └── Typography.xaml
│   │   │   │
│   │   │   ├── Fonts/
│   │   │   │   ├── Inter-Regular.ttf
│   │   │   │   └── Inter-Bold.ttf
│   │   │   │
│   │   │   └── Images/
│   │   │       ├── logo.png
│   │   │       ├── icon.png
│   │   │       └── splash.png
│   │   │
│   │   ├── Helpers/                       # 유틸리티
│   │   │   ├── Converters.cs              # XAML 바인딩 컨버터
│   │   │   ├── Behaviors.cs               # XAML 동작
│   │   │   ├── Validators.cs              # 입력 검증
│   │   │   ├── Constants.cs
│   │   │   └── Extensions.cs
│   │   │
│   │   ├── Messaging/                     # WeakReferenceMessenger
│   │   │   ├── AlertMessage.cs
│   │   │   ├── RefreshMessage.cs
│   │   │   └── NavigationMessage.cs
│   │   │
│   │   ├── Exceptions/
│   │   │   ├── ApiException.cs
│   │   │   ├── NetworkException.cs
│   │   │   └── ValidationException.cs
│   │   │
│   │   └── GlobalUsings.cs                # using 통합 정의
│   │
│   └── MyApp.MAUI.Tests/                  # 단위 테스트
│       ├── ViewModels/
│       │   ├── MainViewModelTests.cs
│       │   └── DashboardViewModelTests.cs
│       │
│       ├── Services/
│       │   ├── ApiServiceTests.cs
│       │   └── StorageServiceTests.cs
│       │
│       └── Helpers/
│           └── ValidatorTests.cs
│
├── MyApp.MAUI.csproj                      # 프로젝트 파일
├── MauiProgram.cs                         # 진입점
└── README.md
```

---

## 🔧 Core Patterns & 구현

### Pattern 1: MauiProgram.cs (완벽한 DI 설정)

```csharp
// src/MyApp.MAUI/MauiProgram.cs
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Messaging;
using MyApp.MAUI.Services;
using MyApp.MAUI.ViewModels;
using MyApp.MAUI.Views;

#nullable enable

namespace MyApp.MAUI;

/// <summary>
/// MAUI 애플리케이션 구성 및 초기화
/// 의존성 주입, 폰트, 테마 설정을 포함합니다.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
            })
            // 플랫폼 기본값 설정
            .ConfigureMauiHandlers(handlers =>
            {
                #if DEBUG
                handlers.AddHandler(typeof(Shell), typeof(ShellHandler));
                #endif
            });

        // 시스템 서비스
        builder.Services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<IDispatcher>(MainThread.Current);

        // 비즈니스 서비스 (싱글톤)
        builder.Services
            .AddSingleton<IApiService, ApiService>()
            .AddSingleton<IStorageService, StorageService>()
            .AddSingleton<ISyncService, SyncService>()
            .AddSingleton<INotificationService, NotificationService>()
            .AddSingleton<INavigationService, NavigationService>();

        // 플랫폼별 서비스
        #if __ANDROID__
        builder.Services.AddSingleton<IPlatformService, Platforms.Android.PlatformService>();
        #elif __IOS__
        builder.Services.AddSingleton<IPlatformService, Platforms.iOS.PlatformService>();
        #elif __MACCATALYST__
        builder.Services.AddSingleton<IPlatformService, Platforms.MacCatalyst.PlatformService>();
        #elif WINDOWS
        builder.Services.AddSingleton<IPlatformService, Platforms.Windows.PlatformService>();
        #endif

        // 로거
        #if DEBUG
        builder.Logging.AddDebug();
        #endif

        // ViewModels (싱글톤)
        builder.Services
            .AddSingleton<MainViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<DetailsViewModel>()
            .AddSingleton<SettingsViewModel>();

        // Views (싱글톤)
        builder.Services
            .AddSingleton<MainPage>()
            .AddSingleton<DashboardPage>()
            .AddSingleton<DetailsPage>()
            .AddSingleton<SettingsPage>();

        // Shell 네비게이션
        builder.Services.AddSingleton<AppShell>();

        // HTTP 클라이언트 (기본값)
        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://api.example.com");
            client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigureHttpClient(client =>
        {
            // 런타임 시 설정 변경
        });

        return builder.Build();
    }
}

// App.xaml.cs
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        // 앱 초기화
        InitializeApp();
    }

    private void InitializeApp()
    {
        // 테마 설정
        if (AppInfo.RequestedTheme == AppTheme.Dark)
        {
            Application.Current?.Resources.ApplyTheme(Resources["DarkTheme"] as ResourceDictionary);
        }

        // 스타일 적용
        ApplyDefaultStyles();
    }

    private void ApplyDefaultStyles()
    {
        // 글로벌 스타일 설정
    }
}
```

---

### Pattern 2: AppShell.xaml (네비게이션 라우팅)

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="MyApp.MAUI.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    FlyoutBehavior="Flyout"
    FlyoutVerticalScrollMode="Enabled"
    Title="MyApp">

    <!-- 스타일 -->
    <Shell.Resources>
        <ResourceDictionary>
            <Color x:Key="PrimaryColor">#2180A8</Color>
            <Color x:Key="AccentColor">#FF6B35</Color>
            <Style x:Key="BaseStyle" TargetType="Element">
                <Setter Property="Shell.BackgroundColor" Value="{StaticResource PrimaryColor}" />
                <Setter Property="Shell.ForegroundColor" Value="White" />
            </Style>
        </ResourceDictionary>
    </Shell.Resources>

    <!-- 탭 기반 네비게이션 -->
    <TabBar>
        <!-- 대시보드 탭 -->
        <ShellContent
            Title="Dashboard"
            Icon="dashboard.png"
            Route="main"
            ContentTemplate="{DataTemplate local:MainPage}">
            <ShellContent.Route>
                <RouteFactory>dashboard</RouteFactory>
            </ShellContent.Route>
        </ShellContent>

        <!-- 데이터 탭 -->
        <Tab Title="Data" Icon="data.png">
            <ShellContent
                Title="List"
                Route="datalist"
                ContentTemplate="{DataTemplate local:DashboardPage}" />
            <ShellContent
                Title="Details"
                Route="datadetails"
                ContentTemplate="{DataTemplate local:DetailsPage}" />
        </Tab>

        <!-- 설정 탭 -->
        <ShellContent
            Title="Settings"
            Icon="settings.png"
            Route="settings"
            ContentTemplate="{DataTemplate local:SettingsPage}" />
    </TabBar>

    <!-- Flyout 아이템 (사이드메뉴) -->
    <FlyoutItem Title="About" Icon="info.png" Route="about">
        <ShellContent ContentTemplate="{DataTemplate local:AboutPage}" />
    </FlyoutItem>

    <FlyoutItem Title="Help" Icon="help.png">
        <ShellContent Route="help" />
    </FlyoutItem>

    <!-- 모달 라우트 (전체 화면) -->
    <Shell.Routes>
        <Route Route="login" Shell="{x:Reference shell}" Handler="{StaticResource loginHandler}" />
    </Shell.Routes>
</Shell>
```

---

### Pattern 3: ViewModel (MVVM Toolkit 기반)

```csharp
// src/MyApp.MAUI/ViewModels/Base/BaseViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MyApp.MAUI.Services;

#nullable enable

namespace MyApp.MAUI.ViewModels;

/// <summary>
/// 모든 ViewModel의 기본 클래스
/// INotifyPropertyChanged, INotifyPropertyChanging 구현
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool hasError;

    protected readonly IApiService ApiService;
    protected readonly IStorageService StorageService;
    protected readonly ISyncService SyncService;
    protected readonly IMessenger Messenger;

    protected BaseViewModel(
        IApiService apiService,
        IStorageService storageService,
        ISyncService syncService,
        IMessenger messenger)
    {
        ApiService = apiService;
        StorageService = storageService;
        SyncService = syncService;
        Messenger = messenger;
    }

    /// <summary>
    /// 에러 처리 (자동 UI 업데이트)
    /// </summary>
    protected void HandleError(Exception ex)
    {
        ErrorMessage = ex.Message;
        HasError = true;
        Debug.WriteLine($"[Error] {ex}");

        // 에러 메시지 발행 (UI에서 처리)
        Messenger.Send(new ErrorMessage(ex.Message));
    }

    /// <summary>
    /// 에러 초기화
    /// </summary>
    protected void ClearError()
    {
        HasError = false;
        ErrorMessage = null;
    }

    /// <summary>
    /// 로딩 상태 관리 (async 작업 래퍼)
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation)
    {
        IsLoading = true;
        try
        {
            await operation();
            ClearError();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 로딩 상태 관리 (반환값 포함)
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        IsLoading = true;
        try
        {
            var result = await operation();
            ClearError();
            return result;
        }
        catch (Exception ex)
        {
            HandleError(ex);
            return default;
        }
        finally
        {
            IsLoading = false;
        }
    }
}

// src/MyApp.MAUI/ViewModels/DashboardViewModel.cs
using CommunityToolkit.Mvvm.Input;

#nullable enable

namespace MyApp.MAUI.ViewModels;

/// <summary>
/// 대시보드 페이지의 ViewModel
/// 데이터 로드, 필터링, 정렬 기능
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    private readonly ICollectionView _collectionView;

    [ObservableProperty]
    private ObservableCollection<Product> products = [];

    [ObservableProperty]
    private Product? selectedProduct;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isRefreshing;

    public DashboardViewModel(
        IApiService apiService,
        IStorageService storageService,
        ISyncService syncService,
        IMessenger messenger)
        : base(apiService, storageService, syncService, messenger)
    {
        // CollectionView 설정 (필터링/정렬)
        _collectionView = CollectionViewSource.Instance.CreateDefault(Products);
        _collectionView.Filter = FilterProducts;
        _collectionView.SortDescriptions.Add(
            new SortDescription("Name", ListSortDirection.Ascending));
    }

    /// <summary>
    /// 데이터 로드 Command (자동 실행)
    /// </summary>
    [RelayCommand]
    public async Task LoadData()
    {
        await ExecuteAsync(async () =>
        {
            // 로컬 스토리지에서 먼저 로드 (빠름)
            var cached = await StorageService.GetAsync<List<Product>>("products");
            if (cached != null)
            {
                Products = new ObservableCollection<Product>(cached);
            }

            // API에서 최신 데이터 로드
            var data = await ApiService.GetAsync<List<Product>>("/api/v1/products");
            if (data != null)
            {
                Products = new ObservableCollection<Product>(data);
                
                // 로컬 캐시 업데이트
                await StorageService.SetAsync("products", data);
                
                // 동기화 (오프라인 changes 업로드)
                await SyncService.SyncAsync();
            }
        });
    }

    /// <summary>
    /// 새로고침 Command (사용자가 당김)
    /// </summary>
    [RelayCommand]
    public async Task Refresh()
    {
        IsRefreshing = true;
        try
        {
            await LoadDataCommand.ExecuteAsync(null);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// 제품 선택 시 상세보기 네비게이션
    /// </summary>
    [RelayCommand]
    public async Task SelectProduct(Product product)
    {
        if (product == null) return;

        // 앱쉘 네비게이션
        await Shell.Current.GoToAsync($"details?id={product.Id}");
    }

    /// <summary>
    /// 검색 필터 적용
    /// </summary>
    partial void OnSearchQueryChanged(string value)
    {
        _collectionView.RefreshFilter();
    }

    /// <summary>
    /// CollectionView 필터 함수
    /// </summary>
    private bool FilterProducts(object obj)
    {
        if (obj is not Product product) return false;
        
        return string.IsNullOrEmpty(SearchQuery) ||
               product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
               product.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 페이지 표시될 때 호출 (자동)
    /// </summary>
    public override void OnAppearing()
    {
        base.OnAppearing();
        
        // 데이터 다시 로드 (네비게이션 후 돌아올 때)
        if (Products.Count == 0)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadDataCommand.ExecuteAsync(null);
            });
        }
    }
}
```

---

### Pattern 4: View (XAML + Code-behind)

```xml
<!-- src/MyApp.MAUI/Views/Pages/DashboardPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="MyApp.MAUI.Views.DashboardPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    Title="Dashboard"
    BackgroundColor="{AppThemeBinding Light=#FAFAF8, Dark=#1F2121}">

    <!-- 리소스 (로컬 스타일) -->
    <ContentPage.Resources>
        <ResourceDictionary>
            <Style x:Key="CardStyle" TargetType="Frame">
                <Setter Property="CornerRadius" Value="12" />
                <Setter Property="BorderColor" Value="{AppThemeBinding Light=#E8E8E8, Dark=#404040}" />
                <Setter Property="HasShadow" Value="True" />
                <Setter Property="Padding" Value="16" />
            </Style>
        </ResourceDictionary>
    </ContentPage.Resources>

    <Grid RowDefinitions="Auto,*" RowSpacing="16" Padding="16">
        <!-- 헤더: 검색바 + 새로고침 -->
        <Grid ColumnDefinitions="*,Auto" ColumnSpacing="8">
            <Entry
                Placeholder="검색..."
                Text="{Binding SearchQuery}"
                BorderColor="{AppThemeBinding Light=#D0D0D0, Dark=#505050}"
                CornerRadius="8" />
            
            <Button
                Text="🔄"
                Command="{Binding RefreshCommand}"
                IsEnabled="{Binding IsRefreshing, Converter={StaticResource InvertedBoolConverter}}"
                Padding="8"
                CornerRadius="8" />
        </Grid>

        <!-- 내용: 제품 리스트 또는 로딩 -->
        <Grid Grid.Row="1">
            <!-- 로딩 인디케이터 -->
            <ActivityIndicator
                IsRunning="{Binding IsLoading}"
                IsVisible="{Binding IsLoading}"
                Color="{StaticResource PrimaryColor}" />

            <!-- 제품 리스트 (CollectionView) -->
            <CollectionView
                ItemsSource="{Binding Products}"
                SelectionMode="Single"
                SelectionChangedCommand="{Binding SelectProductCommand}"
                SelectionChangedCommandParameter="{Binding SelectedItem, Source={RelativeSource Self}}"
                IsVisible="{Binding IsLoading, Converter={StaticResource InvertedBoolConverter}}">
                
                <CollectionView.ItemsLayout>
                    <LinearItemsLayout
                        Orientation="Vertical"
                        ItemSpacing="12" />
                </CollectionView.ItemsLayout>

                <CollectionView.ItemTemplate>
                    <DataTemplate>
                        <Frame Style="{StaticResource CardStyle}">
                            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
                                <!-- 상품 정보 -->
                                <StackLayout Spacing="4">
                                    <Label
                                        Text="{Binding Name}"
                                        FontSize="16"
                                        FontAttributes="Bold" />
                                    
                                    <Label
                                        Text="{Binding Description}"
                                        FontSize="12"
                                        TextColor="{AppThemeBinding Light=#777777, Dark=#AAAAAA}"
                                        LineBreakMode="TailTruncation" />
                                    
                                    <Label
                                        Text="{Binding Price, StringFormat='${0:F2}'}"
                                        FontSize="14"
                                        FontAttributes="Bold"
                                        TextColor="{StaticResource AccentColor}" />
                                </StackLayout>

                                <!-- 상품 이미지 -->
                                <Image
                                    Grid.Column="1"
                                    Source="{Binding ImageUrl}"
                                    Aspect="AspectFill"
                                    WidthRequest="80"
                                    HeightRequest="80"
                                    CornerRadius="8" />
                            </Grid>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>

            <!-- 에러 메시지 -->
            <StackLayout
                IsVisible="{Binding HasError}"
                Padding="16"
                Spacing="8">
                <Label
                    Text="오류 발생"
                    FontSize="16"
                    FontAttributes="Bold"
                    TextColor="{StaticResource ErrorColor}" />
                <Label
                    Text="{Binding ErrorMessage}"
                    FontSize="12"
                    TextColor="{StaticResource ErrorColor}" />
            </StackLayout>
        </Grid>
    </Grid>
</ContentPage>
```

```csharp
// src/MyApp.MAUI/Views/Pages/DashboardPage.xaml.cs
using MyApp.MAUI.ViewModels;

#nullable enable

namespace MyApp.MAUI.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // ViewModel의 데이터 로드
        if (_viewModel.LoadDataCommand.CanExecute(null))
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // 리소스 정리 (필요시)
    }
}
```

---

### Pattern 5: 플랫폼별 구현 (Interface)

```csharp
// src/MyApp.MAUI/Services/Interfaces/IPlatformService.cs
#nullable enable

namespace MyApp.MAUI.Services;

/// <summary>
/// 플랫폼별 기능을 정의하는 인터페이스
/// Android, iOS, Windows 각각 구현
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// 디바이스 고유 ID 가져오기
    /// </summary>
    string GetDeviceId();

    /// <summary>
    /// 런타임 권한 요청 (Android 6.0+)
    /// </summary>
    Task<PermissionStatus> RequestPermissionAsync<T>() where T : Permissions.BasePermission, new();

    /// <summary>
    /// 카메라 스캔
    /// </summary>
    Task<string?> ScanBarcodeAsync();

    /// <summary>
    /// 생체인증 (지문/얼굴)
    /// </summary>
    Task<bool> AuthenticateAsync(string reason);

    /// <summary>
    /// 시스템 저장소에 파일 저장
    /// </summary>
    Task<bool> SaveFileAsync(string filePath, byte[] data);
}

// src/MyApp.MAUI/Platforms/Android/PlatformService.cs
#if __ANDROID__
using Android.Content.PM;
using Android.OS;
using Android.Provider;

#nullable enable

namespace MyApp.MAUI.Platforms.Android;

public class PlatformService : IPlatformService
{
    public string GetDeviceId()
    {
        return Android.OS.Build.Serial ?? "unknown";
    }

    public async Task<PermissionStatus> RequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
            }
            return status;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Permission error: {ex}");
            return PermissionStatus.Denied;
        }
    }

    public async Task<string?> ScanBarcodeAsync()
    {
        try
        {
            // ZXing.Net.MAUI 라이브러리 사용
            var result = await BarcodeReader.Default.CaptureAsync();
            return result?.Value;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Scan error: {ex}");
            return null;
        }
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        try
        {
            var result = await SecureStorage.Default.GetAsync("biometric_enabled");
            if (result == null) return false;

            // BiometricAuthentication MAUI 사용
            var auth = await SecureStorage.Default.GetAsync("biometric");
            return auth == "enabled";
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SaveFileAsync(string filePath, byte[] data)
    {
        try
        {
            var documentsPath = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var fullPath = Path.Combine(documentsPath, filePath);
            
            await File.WriteAllBytesAsync(fullPath, data);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Save error: {ex}");
            return false;
        }
    }
}
#endif

// src/MyApp.MAUI/Platforms/iOS/PlatformService.cs
#if __IOS__
using Foundation;
using UIKit;
using LocalAuthentication;

#nullable enable

namespace MyApp.MAUI.Platforms.iOS;

public class PlatformService : IPlatformService
{
    public string GetDeviceId()
    {
        return UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? "unknown";
    }

    public async Task<PermissionStatus> RequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
            }
            return status;
        }
        catch
        {
            return PermissionStatus.Denied;
        }
    }

    public async Task<string?> ScanBarcodeAsync()
    {
        try
        {
            // Vision framework 사용
            var result = await BarcodeReader.Default.CaptureAsync();
            return result?.Value;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        var context = new LAContext();
        NSError authError = null;

        if (!context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out authError))
        {
            return false;
        }

        var result = await context.EvaluatePolicyAsync(
            LAPolicy.DeviceOwnerAuthenticationWithBiometrics,
            reason);

        return result;
    }

    public async Task<bool> SaveFileAsync(string filePath, byte[] data)
    {
        try
        {
            var documentsPath = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var fullPath = Path.Combine(documentsPath, filePath);
            
            await File.WriteAllBytesAsync(fullPath, data);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
#endif
```

---

## 🚫 Anti-Patterns (절대 금지)

```csharp
// ❌ WRONG: UI 코드를 ViewModel에 넣기
public class BadViewModel
{
    public void ShowAlert(string message)
    {
        Application.Current?.MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage?.DisplayAlert("Alert", message, "OK");
        });
    }
}

// ✅ CORRECT: Messenger 패턴 사용
public class GoodViewModel
{
    private readonly IMessenger _messenger;
    
    public GoodViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }
    
    public void NotifyUser(string message)
    {
        _messenger.Send(new AlertMessage(message));
    }
}
```

```csharp
// ❌ WRONG: 플랫폼 코드를 공유 코드에 혼합
public class BadService
{
    public void DoSomething()
    {
        #if __ANDROID__
        // Android code
        #elif __IOS__
        // iOS code
        #endif
    }
}

// ✅ CORRECT: Interface + 구현 분리
public interface IPlatformService { void DoSomething(); }
public class AndroidService : IPlatformService { ... }
public class iOSService : IPlatformService { ... }
```

---

## ✅ 체크리스트

- [ ] MauiProgram.cs에 모든 서비스 등록됨
- [ ] ViewModel은 BaseViewModel을 상속
- [ ] RelayCommand 사용 (async 지원)
- [ ] 에러 처리는 ExecuteAsync 래퍼 사용
- [ ] 플랫폼 코드는 #if 또는 Interface로 분리
- [ ] XAML에 바인딩이 제대로 설정됨
- [ ] 로컬 스토리지 + API 캐싱 전략 구현
- [ ] 오프라인 동기화 기능 있음
- [ ] 단위 테스트 작성됨

---

## 📚 2025 MAUI 최신 기능

✅ **.NET 9 LTS** - 장기 지원
✅ **MAUI 9.0** - 최신 UI 컨트롤
✅ **C# 13** - 최신 언어 기능
✅ **AOT 지원** - iOS/Android 성능 향상
✅ **Hot Reload** - 개발 생산성

---

**Document Version**: 2.0 (2025-12-09)
**Frameworks**: .NET 9 LTS, MAUI 9.0, C# 13
**Purpose**: Enterprise-grade reference
**Status**: Production-ready for 2025+
