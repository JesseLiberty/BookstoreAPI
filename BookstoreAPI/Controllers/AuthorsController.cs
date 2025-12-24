using BookstoreAPI.Models;
using BookstoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
        try
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors");
            return StatusCode(500, "An error occurred while retrieving authors");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> GetAuthor(int id)
    {
        try
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            
            if (author == null)
                return NotFound();

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving author {AuthorId}", id);
            return StatusCode(500, "An error occurred while retrieving the author");
        }
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByBook(int bookId)
    {
        try
        {
            var authors = await _authorService.GetAuthorsByBookIdAsync(bookId);
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authors for book {BookId}", bookId);
            return StatusCode(500, "An error occurred while retrieving authors");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Author>> CreateAuthor(Author author)
    {
        try
        {
            var createdAuthor = await _authorService.CreateAuthorAsync(author);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating author");
            return StatusCode(500, "An error occurred while creating the author");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, Author author)
    {
        try
        {
            var updatedAuthor = await _authorService.UpdateAuthorAsync(id, author);
            
            if (updatedAuthor == null)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating author {AuthorId}", id);
            return StatusCode(500, "An error occurred while updating the author");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            var deleted = await _authorService.DeleteAuthorAsync(id);
            
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting author {AuthorId}", id);
            return StatusCode(500, "An error occurred while deleting the author");
        }
    }
}
