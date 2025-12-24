using BookstoreAPI.Models;
using BookstoreAPI.Data.Repositories;

namespace BookstoreAPI.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await _authorRepository.GetAllAuthorsAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        return await _authorRepository.GetAuthorByIdAsync(id);
    }

    public async Task<IEnumerable<Author>> GetAuthorsByBookIdAsync(int bookId)
    {
        return await _authorRepository.GetAuthorsByBookIdAsync(bookId);
    }

    public async Task<Author> CreateAuthorAsync(Author author)
    {
        return await _authorRepository.AddAuthorAsync(author);
    }

    public async Task<Author?> UpdateAuthorAsync(int id, Author author)
    {
        var exists = await _authorRepository.AuthorExistsAsync(id);
        if (!exists)
            return null;

        author.Id = id;
        return await _authorRepository.UpdateAuthorAsync(author);
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        return await _authorRepository.DeleteAuthorAsync(id);
    }
}
