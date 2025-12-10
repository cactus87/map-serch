#nullable enable

using LmpLink.MAUI.ViewModels.Base;
using ClosedXML.Excel;
using System.Collections.ObjectModel;

namespace LmpLink.MAUI.ViewModels;

/// <summary>
/// ViewModel for DataPage (Excel import/export, backup/restore).
/// </summary>
public partial class DataViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;
    private readonly IMockDataService _mockDataService;

    // --- Observable Properties ---

    [ObservableProperty]
    private ObservableCollection<Person> _allPersons = new();

    [ObservableProperty]
    private string _lastExportPath = string.Empty;

    [ObservableProperty]
    private string _lastImportPath = string.Empty;

    [ObservableProperty]
    private int _exportedCount;

    [ObservableProperty]
    private int _importedCount;

    // --- Constructor ---

    public DataViewModel(
        ISupabaseService supabaseService,
        IMockDataService mockDataService)
    {
        _supabaseService = supabaseService;
        _mockDataService = mockDataService;
        MauiProgram.Log("[DataViewModel] Constructor called");
    }

    // --- Commands ---

    /// <summary>
    /// Export all persons to Excel file.
    /// </summary>
    [RelayCommand]
    private async Task ExportToExcelAsync()
    {
        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[DataViewModel] ExportToExcelAsync START");

            // Load data from Supabase or mock
            await LoadDataInternalAsync();

            if (AllPersons.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("알림", "내보낼 데이터가 없습니다.", "확인");
                return;
            }

            // Generate filename with timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"LmpLink_Data_{timestamp}.xlsx";
            
            // Get Desktop path
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filePath = Path.Combine(desktopPath, fileName);

            try
            {
                using var workbook = new XLWorkbook();
                
                // Create Users sheet
                var usersSheet = workbook.Worksheets.Add("이용자");
                var users = AllPersons.Where(p => p.Type == PersonType.User).ToList();
                CreateUsersSheet(usersSheet, users);

                // Create Assistants sheet
                var assistantsSheet = workbook.Worksheets.Add("활동지원사");
                var assistants = AllPersons.Where(p => p.Type == PersonType.Assistant).ToList();
                CreateAssistantsSheet(assistantsSheet, assistants);

                // Save to file
                workbook.SaveAs(filePath);

                ExportedCount = AllPersons.Count;
                LastExportPath = filePath;

                MauiProgram.Log($"[DataViewModel] Excel exported: {filePath} ({ExportedCount} rows)");
                
                var result = await Shell.Current.DisplayAlertAsync(
                    "내보내기 완료",
                    $"{ExportedCount}명의 데이터를 내보냈습니다.\n\n파일: {fileName}\n위치: {desktopPath}\n\n파일을 열어보시겠습니까?",
                    "열기",
                    "닫기");

                if (result)
                {
                    // Open file with default application
                    await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[DataViewModel] Export FAILED: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("오류", $"내보내기 실패:\n{ex.Message}", "확인");
            }
        });
    }

    /// <summary>
    /// Import persons from Excel file.
    /// </summary>
    [RelayCommand]
    private async Task ImportFromExcelAsync()
    {
        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[DataViewModel] ImportFromExcelAsync START");

            try
            {
                // Pick Excel file
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".xlsx", ".xls" } },
                        { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                        { DevicePlatform.iOS, new[] { "org.openxmlformats.spreadsheetml.sheet" } }
                    }),
                    PickerTitle = "엑셀 파일을 선택하세요"
                });

                if (result == null)
                {
                    MauiProgram.Log("[DataViewModel] File picker cancelled");
                    return;
                }

                LastImportPath = result.FullPath;
                MauiProgram.Log($"[DataViewModel] Selected file: {LastImportPath}");

                // Read Excel file
                using var stream = await result.OpenReadAsync();
                using var workbook = new XLWorkbook(stream);

                var importedPersons = new List<Person>();

                // Read Users sheet
                if (workbook.Worksheets.Contains("이용자"))
                {
                    var usersSheet = workbook.Worksheet("이용자");
                    var users = ParseUsersSheet(usersSheet);
                    importedPersons.AddRange(users);
                    MauiProgram.Log($"[DataViewModel] Parsed {users.Count} users");
                }

                // Read Assistants sheet
                if (workbook.Worksheets.Contains("활동지원사"))
                {
                    var assistantsSheet = workbook.Worksheet("활동지원사");
                    var assistants = ParseAssistantsSheet(assistantsSheet);
                    importedPersons.AddRange(assistants);
                    MauiProgram.Log($"[DataViewModel] Parsed {assistants.Count} assistants");
                }

                if (importedPersons.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("알림", "가져올 데이터가 없습니다.", "확인");
                    return;
                }

                ImportedCount = importedPersons.Count;

                // Show preview and confirm
                var confirm = await Shell.Current.DisplayAlertAsync(
                    "불러오기 확인",
                    $"{ImportedCount}명의 데이터를 불러왔습니다.\n\nSupabase에 저장하시겠습니까?",
                    "저장",
                    "취소");

                if (!confirm)
                {
                    return;
                }

                // Save to Supabase (TODO: Implement bulk insert)
                // For now, just show success message
                await Shell.Current.DisplayAlertAsync(
                    "알림",
                    "엑셀 불러오기 기능은 구현 중입니다.\nSupabase 벌크 저장 기능은 추후 구현 예정입니다.",
                    "확인");

                MauiProgram.Log($"[DataViewModel] Import completed: {ImportedCount} rows");
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[DataViewModel] Import FAILED: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("오류", $"불러오기 실패:\n{ex.Message}", "확인");
            }
        });
    }

    /// <summary>
    /// Save current data to Supabase.
    /// </summary>
    [RelayCommand]
    private async Task SaveCurrentDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            MauiProgram.Log("[DataViewModel] SaveCurrentDataAsync START");
            
            await Shell.Current.DisplayAlertAsync(
                "알림",
                "현재 데이터 저장 기능은 구현 중입니다.\nSupabase 자동 저장 기능은 추후 구현 예정입니다.",
                "확인");
        });
    }

    // --- Helper Methods ---

    private async Task LoadDataInternalAsync()
    {
        List<Person> users;
        List<Person> assistants;

        try
        {
            await _supabaseService.InitializeAsync();
            users = await _supabaseService.GetUsersAsync();
            assistants = await _supabaseService.GetAssistantsAsync();
            MauiProgram.Log($"[DataViewModel] Loaded from Supabase: {users.Count} users, {assistants.Count} assistants");
        }
        catch (Exception ex)
        {
            MauiProgram.Log($"[DataViewModel] Supabase load failed: {ex.Message}. Using mock data.");
            users = await _mockDataService.GetUsersAsync();
            assistants = await _mockDataService.GetAssistantsAsync();
        }

        AllPersons.Clear();
        foreach (var user in users)
        {
            AllPersons.Add(user);
        }
        foreach (var assistant in assistants)
        {
            AllPersons.Add(assistant);
        }
    }

    private void CreateUsersSheet(IXLWorksheet sheet, List<Person> users)
    {
        // Headers
        sheet.Cell(1, 1).Value = "ID";
        sheet.Cell(1, 2).Value = "이름";
        sheet.Cell(1, 3).Value = "주소";
        sheet.Cell(1, 4).Value = "전화번호";
        sheet.Cell(1, 5).Value = "성별";
        sheet.Cell(1, 6).Value = "위도";
        sheet.Cell(1, 7).Value = "경도";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

        // Data rows
        for (int i = 0; i < users.Count; i++)
        {
            var user = users[i];
            var row = i + 2;
            
            sheet.Cell(row, 1).Value = user.Id;
            sheet.Cell(row, 2).Value = user.Name;
            sheet.Cell(row, 3).Value = user.Address;
            sheet.Cell(row, 4).Value = user.Phone ?? string.Empty;
            sheet.Cell(row, 5).Value = user.Gender ?? string.Empty;
            sheet.Cell(row, 6).Value = user.Latitude;
            sheet.Cell(row, 7).Value = user.Longitude;
        }

        // Auto-fit columns
        sheet.Columns().AdjustToContents();
    }

    private void CreateAssistantsSheet(IXLWorksheet sheet, List<Person> assistants)
    {
        // Headers
        sheet.Cell(1, 1).Value = "ID";
        sheet.Cell(1, 2).Value = "이름";
        sheet.Cell(1, 3).Value = "주소";
        sheet.Cell(1, 4).Value = "전화번호";
        sheet.Cell(1, 5).Value = "성별";
        sheet.Cell(1, 6).Value = "경력(년)";
        sheet.Cell(1, 7).Value = "차량 보유";
        sheet.Cell(1, 8).Value = "근무 시간대";
        sheet.Cell(1, 9).Value = "위도";
        sheet.Cell(1, 10).Value = "경도";

        // Style headers
        var headerRange = sheet.Range(1, 1, 1, 10);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.Orange;

        // Data rows
        for (int i = 0; i < assistants.Count; i++)
        {
            var assistant = assistants[i];
            var row = i + 2;
            
            sheet.Cell(row, 1).Value = assistant.Id;
            sheet.Cell(row, 2).Value = assistant.Name;
            sheet.Cell(row, 3).Value = assistant.Address;
            sheet.Cell(row, 4).Value = assistant.Phone ?? string.Empty;
            sheet.Cell(row, 5).Value = assistant.Gender ?? string.Empty;
            sheet.Cell(row, 6).Value = assistant.Experience ?? 0;
            sheet.Cell(row, 7).Value = assistant.HasVehicle ? "O" : "X";
            sheet.Cell(row, 8).Value = assistant.AvailableTimeSlots ?? string.Empty;
            sheet.Cell(row, 9).Value = assistant.Latitude;
            sheet.Cell(row, 10).Value = assistant.Longitude;
        }

        // Auto-fit columns
        sheet.Columns().AdjustToContents();
    }

    private List<Person> ParseUsersSheet(IXLWorksheet sheet)
    {
        var users = new List<Person>();
        var rows = sheet.RowsUsed().Skip(1); // Skip header

        foreach (var row in rows)
        {
            try
            {
                var user = new Person(
                    Id: (int)row.Cell(1).Value,
                    Name: row.Cell(2).GetString(),
                    Type: PersonType.User,
                    Latitude: (double)row.Cell(6).Value,
                    Longitude: (double)row.Cell(7).Value,
                    Address: row.Cell(3).GetString(),
                    Phone: row.Cell(4).GetString(),
                    Gender: row.Cell(5).GetString()
                );
                users.Add(user);
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[DataViewModel] Failed to parse user row {row.RowNumber()}: {ex.Message}");
            }
        }

        return users;
    }

    private List<Person> ParseAssistantsSheet(IXLWorksheet sheet)
    {
        var assistants = new List<Person>();
        var rows = sheet.RowsUsed().Skip(1); // Skip header

        foreach (var row in rows)
        {
            try
            {
                var assistant = new Person(
                    Id: (int)row.Cell(1).Value,
                    Name: row.Cell(2).GetString(),
                    Type: PersonType.Assistant,
                    Latitude: (double)row.Cell(9).Value,
                    Longitude: (double)row.Cell(10).Value,
                    Address: row.Cell(3).GetString(),
                    Phone: row.Cell(4).GetString(),
                    Gender: row.Cell(5).GetString(),
                    Experience: string.IsNullOrEmpty(row.Cell(6).GetString()) ? null : (int?)row.Cell(6).Value,
                    HasVehicle: row.Cell(7).GetString() == "O",
                    AvailableTimeSlots: row.Cell(8).GetString()
                );
                assistants.Add(assistant);
            }
            catch (Exception ex)
            {
                MauiProgram.Log($"[DataViewModel] Failed to parse assistant row {row.RowNumber()}: {ex.Message}");
            }
        }

        return assistants;
    }
}

