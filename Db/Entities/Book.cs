namespace DotdotTest.Db.Entities;

public class Book : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid AuthorId { get; set; }
    public Author Author { get; set; }
    public int Stock { get; set; }
    public ICollection<BookGenre> BookGenres { get; set; }

}
