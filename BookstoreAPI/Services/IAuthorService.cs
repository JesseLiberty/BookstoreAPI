using BookstoreAPI.Models;

namespace BookstoreAPI.Services;

public interface IAuthorService
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);
    Task<IEnumerable<Author>> GetAuthorsByBookIdAsync(int bookId);
    Task<Author> CreateAuthorAsync(Author author);
    Task<Author?> UpdateAuthorAsync(int id, Author author);
    Task<bool> DeleteAuthorAsync(int id);
}
