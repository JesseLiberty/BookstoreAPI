using BookstoreAPI.Models;
using BookstoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IQueueService _queueService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(
        IBookService bookService,
        IQueueService queueService,
        ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _queueService = queueService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        try
        {
            // Trigger durable function for GET operation via book-get-queue
            await _queueService.SendMessageAsync(
                new { Action = "GetAllBooks", Timestamp = DateTime.UtcNow }, 
                "book-get-queue");
            
            // Still return the data directly for the API response
            var books = await _bookService.GetAllBooksAsync();
            
            _logger.LogInformation("GET request processed, durable function triggered");
            
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving books");
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            
            if (book == null)
                return NotFound();

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book {BookId}", id);
            return StatusCode(500, "An error occurred while retrieving the book");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(BookMessage bookMessage)
    {
        try
        {
            // Send message to book-queue for processing by Azure Function (queue-triggered)
            await _queueService.SendMessageAsync(bookMessage, "book-queue");
            
            _logger.LogInformation("Book creation message queued for Azure Function processing: {Title}", bookMessage.Title);
            
            return Accepted(new { Message = "Book creation request has been queued for processing by Azure Function" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queuing book creation");
            return StatusCode(500, "An error occurred while queuing the book creation");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        try
        {
            var updatedBook = await _bookService.UpdateBookAsync(id, book);
            
            if (updatedBook == null)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book {BookId}", id);
            return StatusCode(500, "An error occurred while updating the book");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book {BookId}", id);
            return StatusCode(500, "An error occurred while deleting the book");
        }
    }
}
