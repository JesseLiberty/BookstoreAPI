using Microsoft.AspNetCore.Mvc;
using BookstoreAPI.Models;
using BookstoreAPI.Services;

namespace BookstoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        _logger.LogInformation("Getting all books");
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        _logger.LogInformation("Getting book with id {Id}", id);
        var book = await _bookService.GetBookByIdAsync(id);
        
        if (book == null)
        {
            _logger.LogWarning("Book with id {Id} not found", id);
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        _logger.LogInformation("Creating new book: {Title}", book.Title);
        var createdBook = await _bookService.CreateBookAsync(book);
        return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Book>> UpdateBook(int id, Book book)
    {
        if (id != book.Id)
        {
            _logger.LogWarning("Book id mismatch: {Id} != {BookId}", id, book.Id);
            return BadRequest("Book ID mismatch");
        }

        _logger.LogInformation("Updating book with id {Id}", id);
        
        try
        {
            var updatedBook = await _bookService.UpdateBookAsync(book);
            return Ok(updatedBook);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Book with id {Id} not found for update", id);
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        _logger.LogInformation("Deleting book with id {Id}", id);
        
        var result = await _bookService.DeleteBookAsync(id);
        if (!result)
        {
            _logger.LogWarning("Book with id {Id} not found for deletion", id);
            return NotFound();
        }

        return NoContent();
    }
}
