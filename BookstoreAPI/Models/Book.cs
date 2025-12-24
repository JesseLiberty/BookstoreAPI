namespace BookstoreAPI.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public DateTime PublishedDate { get; set; }
    public decimal Price { get; set; }
    
    // Navigation property - one book can have many authors
    public ICollection<Author> Authors { get; set; } = new List<Author>();
}
