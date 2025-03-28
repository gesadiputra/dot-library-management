namespace DotdotTest.Model.Book;

public class UpdateBookRequest
{
    public Guid BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Author { get; set; }
    public Guid? AuthorId { get; set; }
    public List<Guid>? GenreIds { get; set; }
    public List<string>? Genres { get; set; }
    public int Stock { get; set; }
}
