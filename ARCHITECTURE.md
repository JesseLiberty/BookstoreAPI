# BookstoreAPI Architecture

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT                                   │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Web API                          │
│                    (BookstoreAPI Project)                        │
│                                                                  │
│  ┌────────────────┐         ┌────────────────┐                 │
│  │  Books         │         │  Authors       │                 │
│  │  Controller    │         │  Controller    │                 │
│  └────────┬───────┘         └────────┬───────┘                 │
│           │                          │                          │
│           │                          │                          │
│  ┌────────▼──────────────────────────▼───────┐                 │
│  │         Service Layer                     │                 │
│  │  - BookService    - AuthorService         │                 │
│  └────────┬──────────────────────────────────┘                 │
│           │                                                     │
│  ┌────────▼──────────────────────────────────┐                 │
│  │      Repository Layer                     │                 │
│  │  - BookRepository  - AuthorRepository     │                 │
│  └────────┬──────────────────────────────────┘                 │
│           │                                                     │
│  ┌────────▼──────────────────────────────────┐                 │
│  │      Queue Service                        │                 │
│  │  - Sends to book-queue (POST)             │                 │
│  │  - Sends to book-get-queue (GET)          │                 │
│  └────────┬──────────────────────────────────┘                 │
└───────────┼──────────────────────────────────────────────────┘
            │
            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    AZURITE (Message Bus)                         │
│                    Azure Storage Emulator                        │
│                                                                  │
│  ┌──────────────────┐         ┌──────────────────┐             │
│  │  book-queue      │         │  book-get-queue  │             │
│  │  (POST msgs)     │         │  (GET msgs)      │             │
│  └────────┬─────────┘         └────────┬─────────┘             │
└───────────┼──────────────────────────────┼────────────────────┘
            │                              │
            ▼                              ▼
┌──────────────────────┐       ┌──────────────────────────┐
│  Azure Function      │       │  Durable Function        │
│  (POST Processing)   │       │  (GET Processing)        │
│                      │       │                          │
│  BookQueueProcessor  │       │  BookDurableFunction     │
│  - Queue Trigger     │       │  - Queue Trigger         │
│  - Creates Books     │       │  - Orchestrator          │
│  - Adds to DB        │       │  - Activity Function     │
└──────────┬───────────┘       └──────────┬───────────────┘
           │                              │
           │                              │
           └──────────────┬───────────────┘
                          ▼
           ┌──────────────────────────────┐
           │      SQL Server Database      │
           │                               │
           │  ┌────────┐    ┌──────────┐  │
           │  │ Books  │    │ Authors  │  │
           │  │ Table  │◄───│  Table   │  │
           │  └────────┘    └──────────┘  │
           │       ▲           (FK: BookId)│
           │       │                       │
           └───────┼───────────────────────┘
                   │
                   │ (EF Core)
                   │
           ┌───────▼───────────────────────┐
           │   BookstoreDbContext          │
           └───────────────────────────────┘
```

## Request Flows

### GET /api/books Flow

```
1. Client → GET /api/books
2. BooksController.GetBooks()
3. ├─→ QueueService.SendMessageAsync("book-get-queue")
4. │   └─→ Azurite Queue (book-get-queue)
5. │       └─→ BookDurableFunction.QueueListener (triggered)
6. │           └─→ BookDurableOrchestrator (started)
7. │               └─→ ProcessGetRequest (activity)
8. │
9. └─→ BookService.GetAllBooksAsync()
    └─→ BookRepository.GetAllBooksAsync()
        └─→ DbContext → SQL Server
            └─→ Returns Books with Authors
                └─→ 200 OK (JSON response to client)
```

### POST /api/books Flow

```
1. Client → POST /api/books { BookMessage }
2. BooksController.CreateBook(bookMessage)
3. QueueService.SendMessageAsync(bookMessage, "book-queue")
4. └─→ Azurite Queue (book-queue)
5.     └─→ BookQueueProcessor.Run (triggered)
6.         ├─→ Creates Book entity
7.         ├─→ DbContext.Books.Add(book)
8.         ├─→ DbContext.SaveChangesAsync()
9.         ├─→ Creates Author entities
10.        ├─→ DbContext.Authors.Add(author)
11.        └─→ DbContext.SaveChangesAsync()
12.            └─→ SQL Server (Book & Authors saved)
13. 
14. API returns → 202 Accepted
```

## Technology Stack

### API Layer
- **ASP.NET Core 10.0** - Web API framework
- **Controllers** - BooksController, AuthorsController

### Business Logic Layer
- **Services** - IBookService, IAuthorService
- **Interfaces** - Dependency injection

### Data Access Layer
- **Repository Pattern** - IBookRepository, IAuthorRepository
- **Entity Framework Core 10.0** - ORM
- **SQL Server** - Database

### Azure Functions
- **Azure Functions Worker 2.51** - Isolated process model
- **Durable Functions 1.12** - Workflow orchestration
- **Queue Triggers** - Event-driven processing

### Message Queue
- **Azurite** - Local Azure Storage Emulator
- **Azure Storage Queues** - Message bus
- **Queue Names**:
  - `book-queue` - POST requests
  - `book-get-queue` - GET requests

### Testing
- **xUnit** - Test framework
- **Moq** - Mocking library
- **InMemory Database** - EF Core testing

## Database Schema

### Books Table
```sql
CREATE TABLE Books (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    ISBN NVARCHAR(20) NOT NULL,
    PublishedDate DATETIME2 NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);
```

### Authors Table
```sql
CREATE TABLE Authors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NULL,
    BookId INT NOT NULL,
    CONSTRAINT FK_Authors_Books FOREIGN KEY (BookId) 
        REFERENCES Books(Id) ON DELETE CASCADE
);
```

## Configuration

### Connection Strings
- **API**: `appsettings.json`
- **Functions**: `local.settings.json`
- **Azurite**: `UseDevelopmentStorage=true`

### Queue Configuration
- **GET Queue**: `book-get-queue`
- **POST Queue**: `book-queue`

## Design Patterns

1. **Repository Pattern** - Data access abstraction
2. **Service Layer** - Business logic separation
3. **Dependency Injection** - Loose coupling
4. **Command Query Separation** - GET uses durable function, POST uses regular function
5. **Asynchronous Processing** - Queue-based messaging
6. **Unit of Work** - DbContext handles transactions
