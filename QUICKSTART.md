# Quick Start Guide

## Prerequisites Check

```bash
# Check .NET SDK
dotnet --version  # Should be 10.0 or later

# Check Node.js (for Azurite)
node --version

# Install Azurite globally
npm install -g azurite
```

## Step-by-Step Startup

### 1. Start Azurite (Terminal 1)
```bash
cd BookstoreAPI
mkdir -p azurite
azurite --silent --location ./azurite --debug ./azurite/debug.log
```

Azurite will start on:
- Blob: http://127.0.0.1:10000
- Queue: http://127.0.0.1:10001 ← **Message bus for our app**
- Table: http://127.0.0.1:10002

### 2. Start Azure Functions (Terminal 2)
```bash
cd BookstoreAPI.Functions

# Copy template if needed
cp local.settings.template.json local.settings.json

# Run functions
dotnet run
```

Functions will listen to:
- `book-queue` - For POST requests
- `book-get-queue` - For GET requests

### 3. Start Web API (Terminal 3)
```bash
cd BookstoreAPI

# Run the API
dotnet run
```

API will start on: http://localhost:5000 (or check console for actual port)

### 4. Test the API

Using curl:
```bash
# GET all books (triggers durable function)
curl http://localhost:5000/api/books

# POST a new book (uses Azure Function)
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Book",
    "isbn": "123-456-789",
    "publishedDate": "2024-01-01T00:00:00",
    "price": 29.99,
    "authors": [
      {
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com"
      }
    ]
  }'

# GET a specific book
curl http://localhost:5000/api/books/1

# GET all authors
curl http://localhost:5000/api/authors

# GET authors for a specific book
curl http://localhost:5000/api/authors/book/1
```

Using the .http file (VS Code with REST Client extension):
```
# Open BookstoreAPI/BookstoreAPI.http
# Click "Send Request" on any request
```

## Troubleshooting

### Database Not Found
```bash
cd BookstoreAPI

# Install EF Core tools
dotnet tool install --global dotnet-ef

# Create/update database
dotnet ef database update
```

### Azurite Not Starting
```bash
# Kill any running instances
pkill azurite

# Clear data and restart
rm -rf azurite
mkdir azurite
azurite --silent --location ./azurite
```

### Functions Not Triggering
1. Check Azurite is running
2. Check connection string in `local.settings.json`: `UseDevelopmentStorage=true`
3. Check function logs for errors
4. Verify queues exist in Azurite (can use Azure Storage Explorer)

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## Running Tests

```bash
cd BookstoreAPI.Tests
dotnet test --verbosity normal
```

Expected output:
```
Passed!  - Failed:     0, Passed:    16, Skipped:     0, Total:    16
```

## Monitoring

### View Queue Messages
Install Azure Storage Explorer to view:
- Queue messages in `book-queue` and `book-get-queue`
- Connection: Use "Attach to local emulator"

### View Function Logs
Azure Functions will output logs to the console showing:
- Queue triggers firing
- Durable function orchestrations
- Database operations

### View API Logs
ASP.NET Core will output logs showing:
- HTTP requests
- Controller actions
- Queue message sends

## Next Steps

1. **Add Swagger/OpenAPI** - Already configured, visit `/openapi` endpoints
2. **Add Authentication** - Implement JWT or OAuth
3. **Add Validation** - Data annotations on models
4. **Add More Tests** - Controller tests, integration tests
5. **Deploy to Azure** - Use Azure App Service + Azure Functions + Azure SQL

## Project Structure

```
BookstoreAPI/
├── BookstoreAPI/              # Web API
│   ├── Controllers/           # API endpoints
│   ├── Models/                # Data models
│   ├── Services/              # Business logic
│   ├── Data/                  # EF Core + Repositories
│   └── Migrations/            # Database migrations
├── BookstoreAPI.Functions/    # Azure Functions
│   ├── BookQueueProcessor.cs  # POST handler
│   └── BookDurableFunction.cs # GET handler
└── BookstoreAPI.Tests/        # Unit tests
    ├── Services/              # Service tests
    └── Repositories/          # Repository tests
```
