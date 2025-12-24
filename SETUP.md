# Bookstore API Setup Guide

## Prerequisites
- .NET 8.0 or later SDK
- SQL Server or LocalDB
- Node.js (for Azurite)
- Azurite (Azure Storage Emulator)

## Setup Instructions

### 1. Install Azurite
```bash
npm install -g azurite
```

### 2. Start Azurite
Run Azurite to provide local Azure Storage emulation (including queues):
```bash
azurite --silent --location ./azurite --debug ./azurite/debug.log
```

This will start:
- Blob service on `http://127.0.0.1:10000`
- Queue service on `http://127.0.0.1:10001` (message bus)
- Table service on `http://127.0.0.1:10002`

### 3. Database Setup
Update the connection string in `appsettings.json` if needed:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookstoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

Create the database:
```bash
cd BookstoreAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application

#### Start the API
```bash
cd BookstoreAPI
dotnet run
```

#### Start the Azure Functions
```bash
cd BookstoreAPI.Functions
func start
```

Or if using `dotnet` directly:
```bash
cd BookstoreAPI.Functions
dotnet run
```

### 5. Run Tests
```bash
cd BookstoreAPI.Tests
dotnet test
```

## Architecture

### Components

1. **ASP.NET Web API** (`BookstoreAPI`)
   - Controllers for Books and Authors
   - Service layer for business logic
   - Repository layer for data access
   - Queue service for message bus integration

2. **Azure Functions** (`BookstoreAPI.Functions`)
   - **GET Durable Function**: Listens to `book-get-queue` via Azurite
   - **POST Azure Function**: Processes book creation from `book-queue`

3. **Database**
   - SQL Server with two tables: `Books` and `Authors`
   - `Authors` has a foreign key to `Books` (BookId)

### Flow

#### GET Request Flow
1. Client → `GET /api/books`
2. API sends message to `book-get-queue` (Azurite)
3. Durable Function reads from queue
4. API returns book data from database

#### POST Request Flow
1. Client → `POST /api/books` with BookMessage
2. API sends message to `book-queue` (Azurite)
3. Azure Function reads from queue
4. Azure Function adds book to database
5. API returns 202 Accepted

## Queue Configuration

- **GET Queue**: `book-get-queue` - Used by Durable Function
- **POST Queue**: `book-queue` - Used by regular Azure Function

Both queues use Azurite as the message bus with connection string:
```
UseDevelopmentStorage=true
```

## API Endpoints

### Books
- `GET /api/books` - Get all books (triggers durable function)
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create book (via Azure Function)
- `PUT /api/books/{id}` - Update book
- `DELETE /api/books/{id}` - Delete book

### Authors
- `GET /api/authors` - Get all authors
- `GET /api/authors/{id}` - Get author by ID
- `GET /api/authors/book/{bookId}` - Get authors by book
- `POST /api/authors` - Create author
- `PUT /api/authors/{id}` - Update author
- `DELETE /api/authors/{id}` - Delete author

## Testing

The solution includes unit tests using xUnit and Moq for:
- Service layer
- Repository layer
- Controllers
