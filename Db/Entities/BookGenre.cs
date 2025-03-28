namespace DotdotTest.Db.Entities;

public class BookGenre : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookId { get; set; }
    public Book Book { get; set; }
    public Guid GenreId { get; set; }
    public Genre Genre { get; set; }
}
