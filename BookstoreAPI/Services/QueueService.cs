using Azure.Storage.Queues;
using System.Text.Json;

namespace BookstoreAPI.Services;

public interface IQueueService
{
    Task SendMessageAsync<T>(T message, string queueName);
}

public class QueueService : IQueueService
{
    private readonly string _connectionString;
    private readonly Dictionary<string, QueueClient> _queueClients = new();

    public QueueService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        if (!_queueClients.ContainsKey(queueName))
        {
            var queueClient = new QueueClient(_connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            _queueClients[queueName] = queueClient;
        }

        var messageJson = JsonSerializer.Serialize(message);
        await _queueClients[queueName].SendMessageAsync(messageJson);
    }
}
