# BookstoreAPI

Demonstration code for series on APIs: https://jesseliberty.com

A complete ASP.NET Core 8.0 Web API for managing a bookstore, featuring Entity Framework Core with SQLite database, repository pattern, and service layer architecture.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Setup Steps](#setup-steps)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Testing the API](#testing-the-api)

## Features

- **RESTful API** with full CRUD operations for books
- **SQLite Database** for lightweight data persistence
- **Repository Pattern** for data access abstraction
- **Service Layer** for business logic separation
- **Swagger/OpenAPI** documentation and interactive API testing
- **Dependency Injection** for loose coupling and testability
- **Logging** with structured logging support
- **Error Handling** with proper HTTP status codes

## Architecture

This application follows a **clean architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                  Controllers Layer                   │
│              (API Endpoints/HTTP)                    │
│                BooksController                       │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                  Service Layer                       │
│              (Business Logic)                        │
│           IBookService/BookService                   │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                Repository Layer                      │
│              (Data Access)                          │
│      IBookRepository/BookRepository                  │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                  Data Layer                         │
│         ApplicationDbContext + EF Core              │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│                SQLite Database                       │
│                 (bookstore.db)                       │
└─────────────────────────────────────────────────────┘
```

### Layer Responsibilities

**Controllers Layer:**
- Handles HTTP requests and responses
- Validates input parameters
- Returns appropriate HTTP status codes
- Delegates business logic to service layer

**Service Layer:**
- Contains business logic
- Orchestrates operations between controllers and repositories
- Provides a clean interface for controllers

**Repository Layer:**
- Abstracts data access operations
- Implements CRUD operations
- Handles Entity Framework queries
- Throws appropriate exceptions for error cases

**Data Layer:**
- Entity Framework Core DbContext
- Database schema configuration
- Entity configurations and constraints

## Prerequisites

- **.NET 8.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download
- **Git** (for cloning the repository)
- **IDE** (optional but recommended):
  - Visual Studio 2022
  - Visual Studio Code with C# extension
  - JetBrains Rider

## Setup Steps

### 1. Clone the Repository

```bash
git clone https://github.com/JesseLiberty/BookstoreAPI.git
cd BookstoreAPI
```

### 2. Verify .NET Installation

```bash
dotnet --version
```

This should display version 8.0 or higher.

### 3. Restore Dependencies

Navigate to the project directory and restore NuGet packages:

```bash
cd BookstoreAPI
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

This will compile the project and verify that all dependencies are correctly installed.

### 5. Database Initialization

The SQLite database is automatically created when the application first runs. The `Program.cs` file includes:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}
```

This ensures the database and tables are created on startup if they don't exist.

## Running the Application

### Start the Development Server

```bash
dotnet run
```

You should see output similar to:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5050
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Access the Application

- **API Base URL:** http://localhost:5050
- **Swagger UI:** http://localhost:5050/swagger

The Swagger UI provides interactive API documentation where you can test all endpoints directly in your browser.

## API Endpoints

All endpoints are prefixed with `/api/books`:

### Get All Books
```http
GET /api/books
```
**Response:** 200 OK with array of books

### Get Book by ID
```http
GET /api/books/{id}
```
**Response:** 
- 200 OK with book object
- 404 Not Found if book doesn't exist

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
**Response:** 201 Created with created book object

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
**Response:**
- 200 OK with updated book object
- 400 Bad Request if ID mismatch
- 404 Not Found if book doesn't exist

### Delete a Book
```http
DELETE /api/books/{id}
```
**Response:**
- 204 No Content on success
- 404 Not Found if book doesn't exist

## Database Schema

The application uses SQLite with the following schema:

### Books Table

| Column            | Type          | Constraints                    |
|-------------------|---------------|--------------------------------|
| Id                | INTEGER       | PRIMARY KEY, AUTOINCREMENT     |
| Title             | TEXT(200)     | NOT NULL                       |
| Author            | TEXT(100)     | NOT NULL                       |
| ISBN              | TEXT(20)      | NOT NULL                       |
| Price             | DECIMAL(18,2) | NOT NULL                       |
| PublicationYear   | INTEGER       | NOT NULL                       |

### Book Model

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int PublicationYear { get; set; }
}
```

## Project Structure

```
BookstoreAPI/
│
├── BookstoreAPI/
│   ├── Controllers/
│   │   └── BooksController.cs          # API endpoints
│   │
│   ├── Data/
│   │   └── ApplicationDbContext.cs     # EF Core DbContext
│   │
│   ├── Models/
│   │   └── Book.cs                     # Book entity
│   │
│   ├── Repositories/
│   │   ├── IBookRepository.cs          # Repository interface
│   │   └── BookRepository.cs           # Repository implementation
│   │
│   ├── Services/
│   │   ├── IBookService.cs             # Service interface
│   │   └── BookService.cs              # Service implementation
│   │
│   ├── Properties/
│   │   └── launchSettings.json         # Launch configuration
│   │
│   ├── BookstoreAPI.csproj             # Project file
│   ├── BookstoreAPI.http               # HTTP test file
│   ├── Program.cs                      # Application entry point
│   ├── appsettings.json                # Configuration
│   └── appsettings.Development.json    # Development configuration
│
├── .gitignore
├── LICENSE
└── README.md
```

## Technologies Used

- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core 8.0** - ORM
- **SQLite** - Embedded database
- **Swashbuckle.AspNetCore 6.6.2** - Swagger/OpenAPI support
- **Microsoft.AspNetCore.OpenApi 8.0.22** - OpenAPI specification

## Testing the API

### Using Swagger UI

1. Navigate to http://localhost:5050/swagger
2. Expand any endpoint
3. Click "Try it out"
4. Fill in the parameters/body
5. Click "Execute"

### Using the Included HTTP File

The project includes a `BookstoreAPI.http` file with sample requests. Open it in Visual Studio Code (with REST Client extension) or Visual Studio to execute requests directly.

### Using cURL

```bash
# Get all books
curl http://localhost:5050/api/books

# Create a book
curl -X POST http://localhost:5050/api/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "1984",
    "author": "George Orwell",
    "isbn": "978-0-452-28423-4",
    "price": 13.99,
    "publicationYear": 1949
  }'

# Get a specific book
curl http://localhost:5050/api/books/1

# Update a book
curl -X PUT http://localhost:5050/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "1984 (Updated)",
    "author": "George Orwell",
    "isbn": "978-0-452-28423-4",
    "price": 14.99,
    "publicationYear": 1949
  }'

# Delete a book
curl -X DELETE http://localhost:5050/api/books/1
```

## Configuration

The application can be configured via `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=bookstore.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Development Notes

- The database file `bookstore.db` is excluded from version control (listed in `.gitignore`)
- The application runs on port 5050 by default (configurable in `launchSettings.json`)
- Logging is configured to output to the console during development
- Swagger UI is only enabled in Development environment

## License

This project is licensed under the MIT License - see the LICENSE file for details.
