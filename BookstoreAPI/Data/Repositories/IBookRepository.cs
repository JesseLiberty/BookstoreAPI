using BookstoreAPI.Models;

namespace BookstoreAPI.Data.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> AddBookAsync(Book book);
    Task<Book?> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(int id);
    Task<bool> BookExistsAsync(int id);
}
