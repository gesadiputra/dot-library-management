namespace DotdotTest.Db.Entities;

public class Genre : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public ICollection<BookGenre> BookGenres { get; set; }
}
