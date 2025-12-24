# BookstoreAPI

Demonstration code for series on APIs: https://jesseliberty.com

A complete ASP.NET Core 8.0 Web API for managing a bookstore, featuring Entity Framework Core with SQLite database.

## Features

- **RESTful API** with full CRUD operations for books
- **SQLite Database** for lightweight data persistence
- **Repository Pattern** for data access abstraction
- **Service Layer** for business logic separation
- **Swagger/OpenAPI** documentation
- **Dependency Injection** for loose coupling

## Architecture

The application follows a clean architecture pattern:

```
BookstoreAPI/
├── Models/           # Data models (Book)
├── Data/            # Database context (ApplicationDbContext)
├── Repositories/    # Data access layer (IBookRepository, BookRepository)
├── Services/        # Business logic layer (IBookService, BookService)
└── Controllers/     # API endpoints (BooksController)
```

## Prerequisites

- .NET 8.0 SDK or later

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/JesseLiberty/BookstoreAPI.git
   cd BookstoreAPI
   ```

2. **Navigate to the project directory**
   ```bash
   cd BookstoreAPI
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   - API Base URL: `http://localhost:5050`
   - Swagger UI: `http://localhost:5050/swagger`

## API Endpoints

### Get All Books
```http
GET /api/books
```

### Get Book by ID
```http
GET /api/books/{id}
```

### Create a New Book
```http
POST /api/books
Content-Type: application/json

{
  "title": "The Great Gatsby",
  "author": "F. Scott Fitzgerald",
  "isbn": "978-0-7432-7356-5",
  "price": 12.99,
  "publicationYear": 1925
}
```

### Update a Book
```http
PUT /api/books/{id}
Content-Type: application/json

{
  "id": 1,
  "title": "The Great Gatsby (Updated)",
  "author": "F. Scott Fitzgerald",
  "isbn": "978-0-7432-7356-5",
  "price": 15.99,
  "publicationYear": 1925
}
```

### Delete a Book
```http
DELETE /api/books/{id}
```

## Book Model

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public decimal Price { get; set; }
    public int PublicationYear { get; set; }
}
```

## Database

The application uses SQLite for data storage. The database file (`bookstore.db`) is automatically created in the project root directory when the application first runs.

### Database Schema

**Books Table:**
- Id (INTEGER, PRIMARY KEY, AUTOINCREMENT)
- Title (TEXT, NOT NULL)
- Author (TEXT, NOT NULL)
- ISBN (TEXT, NOT NULL)
- Price (DECIMAL(18,2), NOT NULL)
- PublicationYear (INTEGER, NOT NULL)

## Testing

You can test the API using:
- **Swagger UI** at `http://localhost:5050/swagger`
- **HTTP file** included in the project (`BookstoreAPI.http`)
- **cURL** or any HTTP client like Postman

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQLite
- Swagger/OpenAPI

## License

This project is licensed under the MIT License - see the LICENSE file for details.
