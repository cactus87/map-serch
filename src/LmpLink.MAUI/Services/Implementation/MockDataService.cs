#nullable enable

namespace LmpLink.MAUI.Services.Implementation;

/// <summary>
/// Mock data service implementation with fixed test data.
/// Center: 도봉구청 (37.6688, 127.0471)
/// Distribution: 0-4km radius
/// </summary>
public class MockDataService : IMockDataService
{
    private const double CenterLat = 37.6688;
    private const double CenterLon = 127.0471;

    private readonly List<Person> _users;
    private readonly List<Person> _assistants;

    public MockDataService()
    {
        _users = GenerateUsers();
        _assistants = GenerateAssistants();
    }

    /// <inheritdoc />
    public Task<List<Person>> GetUsersAsync()
    {
        return Task.FromResult(_users);
    }

    /// <inheritdoc />
    public Task<List<Person>> GetAssistantsAsync()
    {
        return Task.FromResult(_assistants);
    }

    /// <inheritdoc />
    public Task<List<Person>> GetAllPersonsAsync()
    {
        return Task.FromResult(_users.Concat(_assistants).ToList());
    }

    private static List<Person> GenerateUsers()
    {
        return new List<Person>
        {
            new(1, "김영희", PersonType.User, CenterLat + 0.005, CenterLon + 0.003, "서울 도봉구 도봉동 123", "010-1234-5001", "female", false, null, null),
            new(2, "이철수", PersonType.User, CenterLat - 0.008, CenterLon + 0.006, "서울 도봉구 쌍문동 234", "010-1234-5002", "male", false, null, null),
            new(3, "박지영", PersonType.User, CenterLat + 0.012, CenterLon - 0.004, "서울 도봉구 방학동 345", "010-1234-5003", "female", false, null, null),
            new(4, "최민수", PersonType.User, CenterLat - 0.015, CenterLon - 0.008, "서울 도봉구 창동 456", "010-1234-5004", "male", false, null, null),
            new(5, "정수진", PersonType.User, CenterLat + 0.018, CenterLon + 0.010, "서울 강북구 미아동 567", "010-1234-5005", "female", false, null, null),
            new(6, "한동훈", PersonType.User, CenterLat - 0.020, CenterLon + 0.012, "서울 강북구 번동 678", "010-1234-5006", "male", false, null, null),
            new(7, "윤서연", PersonType.User, CenterLat + 0.025, CenterLon - 0.015, "서울 노원구 상계동 789", "010-1234-5007", "female", false, null, null),
            new(8, "강태오", PersonType.User, CenterLat - 0.028, CenterLon - 0.018, "서울 노원구 중계동 890", "010-1234-5008", "male", false, null, null),
            new(9, "임은지", PersonType.User, CenterLat + 0.032, CenterLon + 0.020, "서울 성북구 장위동 901", "010-1234-5009", "female", false, null, null),
            new(10, "조현우", PersonType.User, CenterLat - 0.035, CenterLon + 0.022, "서울 성북구 석관동 012", "010-1234-5010", "male", false, null, null)
        };
    }

    private static List<Person> GenerateAssistants()
    {
        return new List<Person>
        {
            // 반경 0-1km (가까운 거리)
            new(11, "김지원", PersonType.Assistant, CenterLat + 0.003, CenterLon + 0.002, "서울 도봉구 도봉동 111-1", "010-2001-0001", "female", true, "weekday_am", 5),
            new(12, "이민호", PersonType.Assistant, CenterLat - 0.004, CenterLon + 0.003, "서울 도봉구 도봉동 111-2", "010-2001-0002", "male", true, "weekday_pm", 3),
            new(13, "박서현", PersonType.Assistant, CenterLat + 0.005, CenterLon - 0.002, "서울 도봉구 쌍문동 222-1", "010-2001-0003", "female", false, "weekend", 2),
            new(14, "최우진", PersonType.Assistant, CenterLat - 0.006, CenterLon - 0.004, "서울 도봉구 쌍문동 222-2", "010-2001-0004", "male", true, "weekday_am", 4),
            new(15, "정다은", PersonType.Assistant, CenterLat + 0.007, CenterLon + 0.005, "서울 도봉구 방학동 333-1", "010-2001-0005", "female", true, "weekday_pm", 6),

            // 반경 1-2km (중간 거리)
            new(16, "한지훈", PersonType.Assistant, CenterLat - 0.010, CenterLon + 0.008, "서울 도봉구 방학동 333-2", "010-2001-0006", "male", false, "weekend", 1),
            new(17, "윤채원", PersonType.Assistant, CenterLat + 0.012, CenterLon - 0.009, "서울 도봉구 창동 444-1", "010-2001-0007", "female", true, "weekday_am", 5),
            new(18, "강민석", PersonType.Assistant, CenterLat - 0.013, CenterLon - 0.010, "서울 도봉구 창동 444-2", "010-2001-0008", "male", true, "weekday_pm", 4),
            new(19, "임소라", PersonType.Assistant, CenterLat + 0.015, CenterLon + 0.011, "서울 강북구 미아동 555-1", "010-2001-0009", "female", false, "weekend", 7),
            new(20, "조태호", PersonType.Assistant, CenterLat - 0.016, CenterLon + 0.012, "서울 강북구 미아동 555-2", "010-2001-0010", "male", true, "weekday_am", 3),

            // 반경 2-3km
            new(21, "신예린", PersonType.Assistant, CenterLat + 0.020, CenterLon - 0.014, "서울 강북구 번동 666-1", "010-2001-0011", "female", true, "weekday_pm", 8),
            new(22, "배준영", PersonType.Assistant, CenterLat - 0.021, CenterLon - 0.015, "서울 강북구 번동 666-2", "010-2001-0012", "male", false, "weekend", 6),
            new(23, "서유진", PersonType.Assistant, CenterLat + 0.023, CenterLon + 0.016, "서울 노원구 상계동 777-1", "010-2001-0013", "female", true, "weekday_am", 5),
            new(24, "오현수", PersonType.Assistant, CenterLat - 0.024, CenterLon + 0.017, "서울 노원구 상계동 777-2", "010-2001-0014", "male", true, "weekday_pm", 2),
            new(25, "권나영", PersonType.Assistant, CenterLat + 0.026, CenterLon - 0.018, "서울 노원구 중계동 888-1", "010-2001-0015", "female", false, "weekend", 9),

            // 반경 3-4km (먼 거리)
            new(26, "송재민", PersonType.Assistant, CenterLat - 0.028, CenterLon - 0.020, "서울 노원구 중계동 888-2", "010-2001-0016", "male", true, "weekday_am", 4),
            new(27, "홍수아", PersonType.Assistant, CenterLat + 0.030, CenterLon + 0.021, "서울 성북구 장위동 999-1", "010-2001-0017", "female", true, "weekday_pm", 10),
            new(28, "노지훈", PersonType.Assistant, CenterLat - 0.032, CenterLon + 0.023, "서울 성북구 장위동 999-2", "010-2001-0018", "male", false, "weekend", 7),
            new(29, "황지원", PersonType.Assistant, CenterLat + 0.034, CenterLon - 0.024, "서울 성북구 석관동 000-1", "010-2001-0019", "female", true, "weekday_am", 3),
            new(30, "안태양", PersonType.Assistant, CenterLat - 0.036, CenterLon - 0.026, "서울 성북구 석관동 000-2", "010-2001-0020", "male", true, "weekday_pm", 12)
        };
    }
}

