namespace DotdotTest.Model;

public class BasePagedResponse<T>
{
    public int TotalData { get; set; }
    public int ItemCount { get; set; }
    public List<T>? Items { get; set; }
}