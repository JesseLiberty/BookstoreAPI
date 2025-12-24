# BookstoreAPI
Demonstration code for series on APIs: https://jesseliberty.com

## Architecture Overview

This is an ASP.NET Core Web API with a SQL Server backend, using Azure Functions and Azure Storage Queues (via Azurite) for asynchronous processing.

### Key Components

1. **ASP.NET Web API** - RESTful API with controllers
2. **SQL Server Database** - Two tables: Books and Authors
3. **Azure Functions** - Asynchronous processing
   - **Durable Function** for GET requests
   - **Queue-triggered Function** for POST requests
4. **Azure Storage Queues** (via Azurite) - Message bus
5. **Service Layer** - Business logic
6. **Repository Pattern** - Data access layer
7. **Unit Tests** - xUnit with Moq

### Database Schema

- **Books Table**: Id, Title, ISBN, PublishedDate, Price
- **Authors Table**: Id, FirstName, LastName, Email, BookId (FK)

Relationship: One Book can have many Authors

### Request Flow

#### GET /api/books
1. API receives request
2. Sends message to `book-get-queue` (Azurite)
3. **Durable Function** reads from queue and orchestrates processing
4. API returns book data from database

#### POST /api/books
1. API receives BookMessage
2. Sends message to `book-queue` (Azurite)
3. **Azure Function** (queue-triggered) reads from queue
4. Function creates book and authors in database
5. API returns 202 Accepted

## Quick Start

See [SETUP.md](./SETUP.md) for detailed setup instructions.

## Running Tests

```bash
cd BookstoreAPI.Tests
dotnet test
```

All 16 unit tests cover:
- Service layer (8 tests)
- Repository layer (8 tests)
