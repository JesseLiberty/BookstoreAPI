using BookstoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Data.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly BookstoreDbContext _context;

    public AuthorRepository(BookstoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await _context.Authors
            .Include(a => a.Book)
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        return await _context.Authors
            .Include(a => a.Book)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Author>> GetAuthorsByBookIdAsync(int bookId)
    {
        return await _context.Authors
            .Where(a => a.BookId == bookId)
            .ToListAsync();
    }

    public async Task<Author> AddAuthorAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<Author?> UpdateAuthorAsync(Author author)
    {
        var existingAuthor = await _context.Authors.FindAsync(author.Id);
        if (existingAuthor == null)
            return null;

        _context.Entry(existingAuthor).CurrentValues.SetValues(author);
        await _context.SaveChangesAsync();
        return existingAuthor;
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
            return false;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AuthorExistsAsync(int id)
    {
        return await _context.Authors.AnyAsync(a => a.Id == id);
    }
}
