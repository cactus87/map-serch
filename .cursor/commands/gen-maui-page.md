# Slash Command: /maui-page

Generate a new MAUI ContentPage with XAML and code-behind, wired to a ViewModel.

## Usage

```
/maui-page MapPage
/maui-page MatchingPage --with-list
/maui-page SettingsPage --simple
```

## Output Structure

### Views/Pages/MapPage.xaml

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="LmpLink.MAUI.Views.MapPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    Title="지도"
    BackgroundColor="{AppThemeBinding Light=#FFFFFF, Dark=#1F1F1F}">

    <Grid RowDefinitions="Auto,*" RowSpacing="12" Padding="16">
        <!-- 헤더 영역 -->
        <HorizontalStackLayout Spacing="8">
            <Entry
                Placeholder="검색..."
                Text="{Binding SearchQuery}"
                WidthRequest="200" />
            
            <Button
                Text="새로고침"
                Command="{Binding RefreshCommand}"
                IsEnabled="{Binding IsRefreshing, Converter={StaticResource InvertedBoolConverter}}" />
        </HorizontalStackLayout>

        <!-- 메인 콘텐츠 영역 -->
        <Grid Grid.Row="1">
            <!-- 로딩 인디케이터 -->
            <ActivityIndicator
                IsRunning="{Binding IsLoading}"
                IsVisible="{Binding IsLoading}"
                Color="{StaticResource Primary}" />

            <!-- 콘텐츠 (WebView 또는 CollectionView) -->
            <WebView
                x:Name="MapWebView"
                Source="file:///android_asset/map.html"
                IsVisible="{Binding IsLoading, Converter={StaticResource InvertedBoolConverter}}" />

            <!-- 에러 메시지 -->
            <StackLayout
                IsVisible="{Binding HasError}"
                Padding="16"
                Spacing="8"
                VerticalOptions="Center">
                <Label
                    Text="오류 발생"
                    FontSize="16"
                    FontAttributes="Bold"
                    TextColor="{StaticResource Danger}" />
                <Label
                    Text="{Binding ErrorMessage}"
                    FontSize="14"
                    TextColor="{StaticResource Danger}" />
            </StackLayout>
        </Grid>
    </Grid>
</ContentPage>
```

### Views/Pages/MapPage.xaml.cs

```csharp
using LmpLink.MAUI.ViewModels;

#nullable enable

namespace LmpLink.MAUI.Views;

/// <summary>
/// MapPage displays the Naver Map with user/assistant markers.
/// </summary>
public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Load data when page appears
        if (_viewModel.LoadDataCommand.CanExecute(null))
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Cleanup resources if needed
    }

    /// <summary>
    /// Handle WebView navigation events.
    /// </summary>
    private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Success)
        {
            // Initialize map JavaScript
            InitializeMap();
        }
    }

    /// <summary>
    /// Initialize map with markers.
    /// </summary>
    private async void InitializeMap()
    {
        try
        {
            var script = "window.initMap(37.6688, 127.0471, 13);";
            await MapWebView.EvaluateJavaScriptAsync(script);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Map init error: {ex}");
        }
    }
}
```

## Rules

1. **XAML Structure**: Use `Grid` with `RowDefinitions` for flexible layout
2. **Data Binding**: Use `{Binding PropertyName}` for ViewModel properties
3. **Commands**: Use `Command="{Binding CommandName}"` for button actions
4. **Converters**: Use value converters for bool inversion, visibility, etc.
5. **Theme Support**: Use `{AppThemeBinding Light=..., Dark=...}` for colors
6. **Loading State**: Show `ActivityIndicator` when `IsLoading=true`
7. **Error Handling**: Display error message when `HasError=true`
8. **ViewModel Injection**: Inject ViewModel via constructor, set as `BindingContext`
9. **Lifecycle Methods**: Use `OnAppearing`/`OnDisappearing` for load/cleanup
10. **WebView Communication**: Use `EvaluateJavaScriptAsync` for C# → JS calls

## Notes

- **BindingContext**: Set to ViewModel instance for data binding
- **OnAppearing**: Called when page becomes visible (load data here)
- **OnDisappearing**: Called when page hidden (cleanup here)
- **WebView**: Use `file:///android_asset/` (Android) or `ms-appx-web:///` (Windows)
- **Register in MauiProgram.cs**: `builder.Services.AddSingleton<MapPage>()`

## Example Registration (MauiProgram.cs)

```csharp
// Pages (match ViewModel registration order)
builder.Services
    .AddSingleton<MapPage>()
    .AddSingleton<MatchingPage>();
```

## AppShell Navigation

```xml
<!-- AppShell.xaml -->
<TabBar>
    <ShellContent
        Title="지도"
        Icon="map.png"
        Route="map"
        ContentTemplate="{DataTemplate local:MapPage}" />
</TabBar>
```

