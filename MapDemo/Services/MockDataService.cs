using MapDemo.Models;

namespace MapDemo.Services;

public class MockDataService
{
    private readonly List<Person> _people = new();

    public MockDataService()
    {
        InitializeData();
    }

    public List<Person> GetAllPeople() => _people;

    public List<Person> GetUsers() => _people.Where(p => p.Type == PersonType.User).ToList();

    public List<Person> GetAssistants() => _people.Where(p => p.Type == PersonType.Assistant).ToList();

    private void InitializeData()
    {
        // 도봉구청 기준 좌표: 37.6688, 127.0471

        // 이용자 10명 (0~4km 반경 내 분포)
        _people.AddRange(new List<Person>
        {
            // 근거리 (0~1km)
            new Person { Id = "U001", Name = "김철수", Type = PersonType.User, Lat = 37.6700, Lng = 127.0480, Address = "서울시 도봉구 쌍문동" },
            new Person { Id = "U002", Name = "이영희", Type = PersonType.User, Lat = 37.6680, Lng = 127.0490, Address = "서울시 도봉구 방학동" },
            new Person { Id = "U003", Name = "박민수", Type = PersonType.User, Lat = 37.6695, Lng = 127.0460, Address = "서울시 도봉구 창동" },
            
            // 중거리 (1~3km)
            new Person { Id = "U004", Name = "최지우", Type = PersonType.User, Lat = 37.6750, Lng = 127.0500, Address = "서울시 도봉구 도봉동" },
            new Person { Id = "U005", Name = "정수아", Type = PersonType.User, Lat = 37.6650, Lng = 127.0550, Address = "서울시 도봉구 쌍문1동" },
            new Person { Id = "U006", Name = "강동원", Type = PersonType.User, Lat = 37.6800, Lng = 127.0450, Address = "서울시 도봉구 방학1동" },
            new Person { Id = "U007", Name = "조인성", Type = PersonType.User, Lat = 37.6620, Lng = 127.0400, Address = "서울시 도봉구 창1동" },
            
            // 원거리 (3~4km)
            new Person { Id = "U008", Name = "송혜교", Type = PersonType.User, Lat = 37.6850, Lng = 127.0600, Address = "서울시 도봉구 도봉1동" },
            new Person { Id = "U009", Name = "전지현", Type = PersonType.User, Lat = 37.6550, Lng = 127.0350, Address = "서울시 도봉구 쌍문2동" },
            new Person { Id = "U010", Name = "이병헌", Type = PersonType.User, Lat = 37.6900, Lng = 127.0500, Address = "서울시 도봉구 방학2동" }
        });

        // 활동지원사 20명 (0~4km 반경 내 분포)
        _people.AddRange(new List<Person>
        {
            // 근거리 (0~1km) - 6명
            new Person { Id = "A001", Name = "지원사1", Type = PersonType.Assistant, Lat = 37.6690, Lng = 127.0475, Address = "서울시 도봉구 쌍문동" },
            new Person { Id = "A002", Name = "지원사2", Type = PersonType.Assistant, Lat = 37.6685, Lng = 127.0485, Address = "서울시 도봉구 방학동" },
            new Person { Id = "A003", Name = "지원사3", Type = PersonType.Assistant, Lat = 37.6705, Lng = 127.0465, Address = "서울시 도봉구 창동" },
            new Person { Id = "A004", Name = "지원사4", Type = PersonType.Assistant, Lat = 37.6695, Lng = 127.0470, Address = "서울시 도봉구 도봉동" },
            new Person { Id = "A005", Name = "지원사5", Type = PersonType.Assistant, Lat = 37.6680, Lng = 127.0480, Address = "서울시 도봉구 쌍문1동" },
            new Person { Id = "A006", Name = "지원사6", Type = PersonType.Assistant, Lat = 37.6700, Lng = 127.0460, Address = "서울시 도봉구 방학1동" },
            
            // 중거리 (1~3km) - 10명
            new Person { Id = "A007", Name = "지원사7", Type = PersonType.Assistant, Lat = 37.6730, Lng = 127.0510, Address = "서울시 도봉구 창1동" },
            new Person { Id = "A008", Name = "지원사8", Type = PersonType.Assistant, Lat = 37.6660, Lng = 127.0530, Address = "서울시 도봉구 도봉1동" },
            new Person { Id = "A009", Name = "지원사9", Type = PersonType.Assistant, Lat = 37.6770, Lng = 127.0490, Address = "서울시 도봉구 쌍문2동" },
            new Person { Id = "A010", Name = "지원사10", Type = PersonType.Assistant, Lat = 37.6640, Lng = 127.0420, Address = "서울시 도봉구 방학2동" },
            new Person { Id = "A011", Name = "지원사11", Type = PersonType.Assistant, Lat = 37.6780, Lng = 127.0460, Address = "서울시 도봉구 창2동" },
            new Person { Id = "A012", Name = "지원사12", Type = PersonType.Assistant, Lat = 37.6630, Lng = 127.0540, Address = "서울시 도봉구 도봉2동" },
            new Person { Id = "A013", Name = "지원사13", Type = PersonType.Assistant, Lat = 37.6810, Lng = 127.0440, Address = "서울시 도봉구 쌍문3동" },
            new Person { Id = "A014", Name = "지원사14", Type = PersonType.Assistant, Lat = 37.6610, Lng = 127.0410, Address = "서울시 도봉구 방학3동" },
            new Person { Id = "A015", Name = "지원사15", Type = PersonType.Assistant, Lat = 37.6750, Lng = 127.0520, Address = "서울시 도봉구 창3동" },
            new Person { Id = "A016", Name = "지원사16", Type = PersonType.Assistant, Lat = 37.6670, Lng = 127.0390, Address = "서울시 도봉구 도봉3동" },
            
            // 원거리 (3~4km) - 4명
            new Person { Id = "A017", Name = "지원사17", Type = PersonType.Assistant, Lat = 37.6860, Lng = 127.0590, Address = "서울시 도봉구 쌍문4동" },
            new Person { Id = "A018", Name = "지원사18", Type = PersonType.Assistant, Lat = 37.6540, Lng = 127.0360, Address = "서울시 도봉구 방학4동" },
            new Person { Id = "A019", Name = "지원사19", Type = PersonType.Assistant, Lat = 37.6910, Lng = 127.0510, Address = "서울시 도봉구 창4동" },
            new Person { Id = "A020", Name = "지원사20", Type = PersonType.Assistant, Lat = 37.6520, Lng = 127.0340, Address = "서울시 도봉구 도봉4동" }
        });
    }
}
