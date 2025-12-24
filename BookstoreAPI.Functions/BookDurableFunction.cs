using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace BookstoreAPI.Functions;

/// <summary>
/// Durable Function for processing GET requests
/// </summary>
public class BookDurableFunction
{
    private readonly ILogger<BookDurableFunction> _logger;

    public BookDurableFunction(ILogger<BookDurableFunction> logger)
    {
        _logger = logger;
    }

    [Function("BookDurableOrchestrator")]
    public async Task<string> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<string>();
        _logger.LogInformation("Durable Function Orchestrator started for GET request with input: {Input}", input);

        var result = await context.CallActivityAsync<string>("ProcessGetRequest", input);
        
        _logger.LogInformation("Durable Function Orchestrator completed");
        
        return result;
    }

    [Function("ProcessGetRequest")]
    public string ProcessGetRequest([ActivityTrigger] string input, FunctionContext context)
    {
        _logger.LogInformation("Durable Function Activity: Processing GET request: {Input}", input);
        
        // This activity function can perform any necessary processing for GET requests
        // For example: logging, auditing, triggering notifications, etc.
        
        return $"Durable Function processed GET request at {DateTime.UtcNow}";
    }

    [Function("BookGetQueueListener")]
    public async Task<string> QueueListener(
        [QueueTrigger("book-get-queue", Connection = "AzureWebJobsStorage")] string queueMessage,
        [DurableClient] DurableTaskClient durableClient,
        FunctionContext context)
    {
        _logger.LogInformation("Durable Function queue trigger received for GET: {Message}", queueMessage);

        // Start the durable orchestrator for GET requests
        string instanceId = await durableClient.ScheduleNewOrchestrationInstanceAsync(
            "BookDurableOrchestrator", 
            queueMessage);

        _logger.LogInformation("Started durable orchestration with ID = '{InstanceId}'", instanceId);

        return $"Started durable orchestration: {instanceId}";
    }
}
