using BookstoreAPI.Models;
using BookstoreAPI.Data.Repositories;

namespace BookstoreAPI.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllBooksAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetBookByIdAsync(id);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        return await _bookRepository.AddBookAsync(book);
    }

    public async Task<Book?> UpdateBookAsync(int id, Book book)
    {
        var exists = await _bookRepository.BookExistsAsync(id);
        if (!exists)
            return null;

        book.Id = id;
        return await _bookRepository.UpdateBookAsync(book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        return await _bookRepository.DeleteBookAsync(id);
    }
}
