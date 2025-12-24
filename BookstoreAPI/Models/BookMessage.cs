namespace BookstoreAPI.Models;

public class BookMessage
{
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public DateTime PublishedDate { get; set; }
    public decimal Price { get; set; }
    public List<AuthorData> Authors { get; set; } = new();
}

public class AuthorData
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
}
