namespace MapDemo.Models;

public class Person
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public PersonType Type { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Address { get; set; } = string.Empty;
}

public enum PersonType
{
    User,      // 이용자
    Assistant  // 활동지원사
}
