using BookstoreAPI.Data;
using BookstoreAPI.Data.Repositories;
using BookstoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookstoreAPI.Tests.Repositories;

public class BookRepositoryTests
{
    private BookstoreDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BookstoreDbContext(options);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        
        context.Books.AddRange(
            new Book { Title = "Book 1", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m },
            new Book { Title = "Book 2", ISBN = "456", PublishedDate = DateTime.Now, Price = 20.99m }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllBooksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        
        var book = new Book { Title = "Test Book", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetBookByIdAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
    }

    [Fact]
    public async Task AddBookAsync_AddsBook()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        var book = new Book { Title = "New Book", ISBN = "789", PublishedDate = DateTime.Now, Price = 15.99m };

        // Act
        var result = await repository.AddBookAsync(book);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(1, await context.Books.CountAsync());
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesExistingBook()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        
        var book = new Book { Title = "Original Title", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        book.Title = "Updated Title";
        var result = await repository.UpdateBookAsync(book);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        
        var updatedBook = await context.Books.FindAsync(book.Id);
        Assert.Equal("Updated Title", updatedBook?.Title);
    }

    [Fact]
    public async Task DeleteBookAsync_DeletesBook_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        
        var book = new Book { Title = "Book to Delete", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteBookAsync(book.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await context.Books.CountAsync());
    }

    [Fact]
    public async Task DeleteBookAsync_ReturnsFalse_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);

        // Act
        var result = await repository.DeleteBookAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task BookExistsAsync_ReturnsTrue_WhenBookExists()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);
        
        var book = new Book { Title = "Test Book", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.BookExistsAsync(book.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task BookExistsAsync_ReturnsFalse_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new BookRepository(context);

        // Act
        var result = await repository.BookExistsAsync(999);

        // Assert
        Assert.False(result);
    }
}
