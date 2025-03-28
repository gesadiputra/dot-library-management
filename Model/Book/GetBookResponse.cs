namespace DotdotTest.Model.Book;

public class GetBookResponse
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; }
    public string BookDescription { get; set; }
    public GetAuthorResponse Author { get; set; }
    public List<GetGenreResponse> Genres { get; set; }

}

public class GetAuthorResponse
{
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
}

public class GetGenreResponse
{
    public Guid GenreId { get; set; }
    public string GenreName { get; set; }
}