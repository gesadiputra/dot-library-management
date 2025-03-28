namespace DotdotTest.Model;

public class BasePagedRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Search { get; set; }
    public int SkipCount()
        => (Page - 1) * Size;
}