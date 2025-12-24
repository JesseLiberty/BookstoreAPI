using BookstoreAPI.Data.Repositories;
using BookstoreAPI.Models;
using BookstoreAPI.Services;
using Moq;
using Xunit;

namespace BookstoreAPI.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockRepository;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _mockRepository = new Mock<IBookRepository>();
        _service = new BookService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        var expectedBooks = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m },
            new Book { Id = 2, Title = "Test Book 2", ISBN = "456", PublishedDate = DateTime.Now, Price = 20.99m }
        };
        _mockRepository.Setup(r => r.GetAllBooksAsync()).ReturnsAsync(expectedBooks);

        // Act
        var result = await _service.GetAllBooksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllBooksAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var expectedBook = new Book { Id = 1, Title = "Test Book", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        _mockRepository.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(expectedBook);

        // Act
        var result = await _service.GetBookByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Book", result.Title);
        _mockRepository.Verify(r => r.GetBookByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetBookByIdAsync(999)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.GetBookByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetBookByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateBookAsync_CreatesAndReturnsBook()
    {
        // Arrange
        var newBook = new Book { Title = "New Book", ISBN = "789", PublishedDate = DateTime.Now, Price = 15.99m };
        _mockRepository.Setup(r => r.AddBookAsync(It.IsAny<Book>())).ReturnsAsync(newBook);

        // Act
        var result = await _service.CreateBookAsync(newBook);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Book", result.Title);
        _mockRepository.Verify(r => r.AddBookAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesBook_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Updated Book", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        _mockRepository.Setup(r => r.BookExistsAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateBookAsync(It.IsAny<Book>())).ReturnsAsync(book);

        // Act
        var result = await _service.UpdateBookAsync(1, book);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Book", result.Title);
        _mockRepository.Verify(r => r.BookExistsAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateBookAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        var book = new Book { Id = 999, Title = "Updated Book", ISBN = "123", PublishedDate = DateTime.Now, Price = 10.99m };
        _mockRepository.Setup(r => r.BookExistsAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateBookAsync(999, book);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.BookExistsAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateBookAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookAsync_ReturnsTrue_WhenBookIsDeleted()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteBookAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteBookAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteBookAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_ReturnsFalse_WhenBookDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteBookAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteBookAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteBookAsync(999), Times.Once);
    }
}
