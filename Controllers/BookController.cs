using DotdotTest.Model.Book;
using DotdotTest.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotdotTest.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> CreateBook(CreateBookRequest request)
    {
        if (request.AuthorId == null && string.IsNullOrEmpty(request.Author))
            throw new Exception("Please insert Author!");
        if (request.GenreIds == null && request.Genres == null)
            throw new Exception("Please insert Genre!");
        if ((request.GenreIds != null && !request.GenreIds.Any()) || (request.Genres != null && !request.Genres.Any()))
            throw new Exception("Genre must be at least 1!");

        await _bookService.CreateBook(request);
        return Ok();
    }

    [HttpPut("")]
    [Authorize]
    public async Task<IActionResult> UpdateBook(UpdateBookRequest request)
    {
        /*if (request.AuthorId == null && string.IsNullOrEmpty(request.Author))
            throw new Exception("Please insert Author!");
        if (request.GenreIds == null && request.Genres == null)
            throw new Exception("Please insert Genre!");
        if ((request.GenreIds != null && !request.GenreIds.Any()) || (request.Genres != null && !request.Genres.Any()))
            throw new Exception("Genre must be at least 1!");*/

        await _bookService.UpdateBook(request);
        return Ok();
    }

    [HttpDelete("")]
    [Authorize]
    public async Task<IActionResult> DeleteBook(BookIdRequest request)
    {
        await _bookService.DeleteBook(request.BookId);
        return Ok();
    }

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetPaginatedBook([FromQuery] GetPagedBookRequest request)
    {
        var result = await _bookService.GetPaginatedBook(request);
        return Ok(result);
    }

    [HttpGet("{bookId}")]
    [Authorize]
    public async Task<IActionResult> GetDetailBook([FromRoute] Guid bookId)
    {
        var result = await _bookService.GetDetailBook(bookId);
        return Ok(result);
    }

    [HttpGet("author")]
    [Authorize]
    public async Task<IActionResult> GetAllAuthors()
    {
        var result = await _bookService.GetAllAuthor();
        return Ok(result);
    }

    [HttpGet("genre")]
    [Authorize]
    public async Task<IActionResult> GetAllGenres()
    {
        var result = await _bookService.GetAllGenre();
        return Ok(result);
    }
}
