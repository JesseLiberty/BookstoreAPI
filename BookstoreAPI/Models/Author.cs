namespace BookstoreAPI.Models;

public class Author
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    
    // Foreign key
    public int BookId { get; set; }
    
    // Navigation property
    public Book? Book { get; set; }
}
