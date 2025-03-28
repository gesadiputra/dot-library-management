using DotdotTest.Db;
using DotdotTest.Db.Entities;
using DotdotTest.Model;
using DotdotTest.Model.Book;
using Microsoft.EntityFrameworkCore;

namespace DotdotTest.Service;

public interface IBookService
{
    Task CreateBook(CreateBookRequest request);
    Task UpdateBook(UpdateBookRequest request);
    Task DeleteBook(Guid bookId);
    Task<BasePagedResponse<GetBookResponse>> GetPaginatedBook(GetPagedBookRequest request);
    Task<GetBookResponse> GetDetailBook(Guid bookId);
    Task<List<GetAuthorResponse>> GetAllAuthor();
    Task<List<GetGenreResponse>> GetAllGenre();
}
public class BookService : IBookService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAuthService _authService;
    public BookService(ApplicationDbContext dbContext, IAuthService authService)
    {
        _dbContext = dbContext;
        _authService = authService;
    }

    private async Task<Book> GetBook(Guid bookId)
    {
        var book = await _dbContext
            .Set<Book>()
            .Where(p => p.Id == bookId)
            .FirstOrDefaultAsync();
        if (book == null) throw new Exception("Book not found");
        return book;
    }

    private async Task<List<BookGenre>> GetBookGenres(Guid bookId)
    {
        return await _dbContext
            .Set<BookGenre>()
            .Where(p => p.BookId == bookId)
            .ToListAsync();
    }
    public async Task CreateBook(CreateBookRequest request)
    {
        var currentUser = _authService.GetUserFromToken();
        var now = DateTime.UtcNow;
        var book = new Book
        {
            Title = request.Title,
            Description = request.Description,
            Stock = request.Stock,
            CreatedBy = currentUser.Id,
            CreatedDate = now
        };

        if (request.AuthorId != null)
        {
            var author = await _dbContext
                .Set<Author>()
                .Where(p => p.Id == request.AuthorId)
                .FirstOrDefaultAsync();
            if (author == null) throw new Exception("Author not found!");

            book.AuthorId = author.Id;
        }
        else
        {
            var author = new Author
            {
                Name = request.Author!,
                CreatedBy = currentUser.Id,
                CreatedDate = now
            };
            await _dbContext.AddAsync(author);

            book.AuthorId = author.Id;
        }

        var bookGenres = new List<BookGenre>();
        if (request.GenreIds != null)
        {
            var genres = await _dbContext
                .Set<Genre>()
                .Where(p => request.GenreIds.Contains(p.Id))
                .ToListAsync();
            if (!genres.Any())
                throw new Exception("No valid Genre found!");

            foreach(var genre in genres)
            {
                var bookGenre = new BookGenre
                {
                    GenreId = genre.Id,
                    BookId = book.Id,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                bookGenres.Add(bookGenre);
            }
        }
        
        if (request.Genres != null)
        {
            foreach(var genre in request.Genres)
            {
                var newGenre = new Genre
                {
                    Name = genre,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                await _dbContext.AddAsync(newGenre);

                var bookGenre = new BookGenre
                {
                    GenreId = newGenre.Id,
                    BookId = book.Id,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                bookGenres.Add(bookGenre);
            }
        }

        await _dbContext.AddAsync(book);
        await _dbContext.AddRangeAsync(bookGenres);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBook(UpdateBookRequest request)
    {
        var currentUser = _authService.GetUserFromToken();
        var now = DateTime.UtcNow;

        var book = await GetBook(request.BookId);
        _dbContext.Attach(book);
        book.Title = request.Title;
        book.Description = request.Description;
        book.Stock = request.Stock;
        book.UpdatedBy = currentUser.Id;
        book.UpdatedDate = now;

        if (request.AuthorId != null)
        {
            var author = await _dbContext
                .Set<Author>()
                .Where(p => p.Id == request.AuthorId)
                .FirstOrDefaultAsync();
            if (author == null) throw new Exception("Author not found!");

            book.AuthorId = author.Id;
        }
        else
        {
            var author = new Author
            {
                Name = request.Author!,
                CreatedBy = currentUser.Id,
                CreatedDate = now
            };
            await _dbContext.AddAsync(author);

            book.AuthorId = author.Id;
        }

        var existingBookGenres = await GetBookGenres(request.BookId);
        _dbContext.RemoveRange(existingBookGenres);

        var bookGenres = new List<BookGenre>();
        if (request.GenreIds != null)
        {
            var genres = await _dbContext
                .Set<Genre>()
                .Where(p => request.GenreIds.Contains(p.Id))
                .ToListAsync();
            if (!genres.Any())
                throw new Exception("No valid Genre found!");

            foreach (var genre in genres)
            {
                var bookGenre = new BookGenre
                {
                    GenreId = genre.Id,
                    BookId = book.Id,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                bookGenres.Add(bookGenre);
            }
        }

        if (request.Genres != null)
        {
            foreach (var genre in request.Genres)
            {
                var newGenre = new Genre
                {
                    Name = genre,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                await _dbContext.AddAsync(newGenre);

                var bookGenre = new BookGenre
                {
                    GenreId = newGenre.Id,
                    BookId = book.Id,
                    CreatedBy = currentUser.Id,
                    CreatedDate = now
                };
                bookGenres.Add(bookGenre);
            }
        }

        _dbContext.Update(book);
        await _dbContext.AddRangeAsync(bookGenres);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteBook(Guid bookId)
    {
        var book = await GetBook(bookId);

        var bookGenres = await GetBookGenres(bookId);
        _dbContext.RemoveRange(bookGenres);

        _dbContext.Remove(book);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<BasePagedResponse<GetBookResponse>> GetPaginatedBook(GetPagedBookRequest request)
    {
        var query = _dbContext
            .Set<Book>()
            .Include(p => p.Author)
            .Include(p => p.BookGenres).ThenInclude(p => p.Genre)
            .Select(p => new GetBookResponse
            {
                BookId = p.Id,
                BookTitle = p.Title,
                BookDescription = p.Description,
                Author = new GetAuthorResponse
                {
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name
                },
                Genres = p.BookGenres.Select(q => new GetGenreResponse
                {
                    GenreId = q.GenreId,
                    GenreName = q.Genre.Name
                }).ToList()
            })
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => p.BookTitle.Contains(request.Search) || p.Author.AuthorName.Contains(request.Search));
        }

        var totalData = await query.CountAsync();
        var result = await query
            .Skip(request.SkipCount())
            .Take(request.Size)
            .ToListAsync();

        return new BasePagedResponse<GetBookResponse>
        {
            TotalData = totalData,
            ItemCount = result.Count,
            Items = result
        };
    }

    public async Task<GetBookResponse> GetDetailBook(Guid bookId)
    {
        var result = await _dbContext
            .Set<Book>()
            .Include(p => p.Author)
            .Include(p => p.BookGenres).ThenInclude(p => p.Genre)
            .Where(p => p.Id == bookId)
            .Select(p => new GetBookResponse
            {
                BookId = p.Id,
                BookTitle = p.Title,
                BookDescription = p.Description,
                Author = new GetAuthorResponse
                {
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name
                },
                Genres = p.BookGenres.Select(q => new GetGenreResponse
                {
                    GenreId = q.GenreId,
                    GenreName = q.Genre.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
        if (result == null) throw new Exception("Book Not Found");
        return result;
    }

    public async Task<List<GetAuthorResponse>> GetAllAuthor()
    {
        return await _dbContext
            .Set<Author>()
            .Select(p => new GetAuthorResponse
            {
                AuthorId = p.Id,
                AuthorName = p.Name
            })
            .ToListAsync();
    }

    public async Task<List<GetGenreResponse>> GetAllGenre()
    {
        return await _dbContext
            .Set<Genre>()
            .Select(p => new GetGenreResponse
            {
                GenreId = p.Id,
                GenreName = p.Name
            })
            .ToListAsync();
    }
}
