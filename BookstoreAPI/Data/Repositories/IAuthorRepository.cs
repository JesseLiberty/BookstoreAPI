using BookstoreAPI.Models;

namespace BookstoreAPI.Data.Repositories;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);
    Task<IEnumerable<Author>> GetAuthorsByBookIdAsync(int bookId);
    Task<Author> AddAuthorAsync(Author author);
    Task<Author?> UpdateAuthorAsync(Author author);
    Task<bool> DeleteAuthorAsync(int id);
    Task<bool> AuthorExistsAsync(int id);
}
