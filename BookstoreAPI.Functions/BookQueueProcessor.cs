using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BookstoreAPI.Models;
using BookstoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Functions;

/// <summary>
/// Azure Function (Queue-triggered) for processing POST requests to create books
/// </summary>
public class BookQueueProcessor
{
    private readonly ILogger<BookQueueProcessor> _logger;

    public BookQueueProcessor(ILogger<BookQueueProcessor> logger)
    {
        _logger = logger;
    }

    [Function("ProcessBookCreation")]
    public async Task Run(
        [QueueTrigger("book-queue", Connection = "AzureWebJobsStorage")] string queueMessage,
        FunctionContext context)
    {
        _logger.LogInformation("Azure Function processing POST book creation message: {Message}", queueMessage);

        try
        {
            var bookMessage = JsonSerializer.Deserialize<BookMessage>(queueMessage);
            
            if (bookMessage == null)
            {
                _logger.LogError("Failed to deserialize book message");
                return;
            }

            // Get the DbContext from the service provider
            var dbContextFactory = context.InstanceServices.GetService(typeof(IDbContextFactory<BookstoreDbContext>)) 
                as IDbContextFactory<BookstoreDbContext>;
            
            if (dbContextFactory != null)
            {
                await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                
                // Create the book entity
                var book = new Book
                {
                    Title = bookMessage.Title,
                    ISBN = bookMessage.ISBN,
                    PublishedDate = bookMessage.PublishedDate,
                    Price = bookMessage.Price
                };

                // Add the book to the database
                dbContext.Books.Add(book);
                await dbContext.SaveChangesAsync();

                // Add authors if any
                if (bookMessage.Authors.Any())
                {
                    foreach (var authorData in bookMessage.Authors)
                    {
                        var author = new Author
                        {
                            FirstName = authorData.FirstName,
                            LastName = authorData.LastName,
                            Email = authorData.Email,
                            BookId = book.Id
                        };
                        dbContext.Authors.Add(author);
                    }
                    await dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Azure Function successfully created book: {BookId} - {Title}", book.Id, book.Title);
            }
            else
            {
                _logger.LogError("DbContextFactory not available");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Azure Function processing book creation message");
            throw;
        }
    }
}
