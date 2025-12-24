using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookstoreAPI.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure DbContext with SQL Server
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=BookstoreDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        services.AddDbContextFactory<BookstoreDbContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();

host.Run();
