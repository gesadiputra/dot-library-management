namespace DotdotTest.Db.Entities;

public class Author : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
}
