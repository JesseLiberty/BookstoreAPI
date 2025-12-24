using BookstoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookstoreDbContext _context;

    public BookRepository(BookstoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .Include(b => b.Authors)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> UpdateBookAsync(Book book)
    {
        var existingBook = await _context.Books.FindAsync(book.Id);
        if (existingBook == null)
            return null;

        _context.Entry(existingBook).CurrentValues.SetValues(book);
        await _context.SaveChangesAsync();
        return existingBook;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BookExistsAsync(int id)
    {
        return await _context.Books.AnyAsync(b => b.Id == id);
    }
}
