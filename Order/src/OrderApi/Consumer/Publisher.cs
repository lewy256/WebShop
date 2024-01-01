using Azure.Messaging.ServiceBus;
using OrderApi.Shared;
using System.Text.Json;

namespace OrderApi.Consumer;

public class Publisher {
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _clientSender;

    public Publisher(IConfiguration configuration) {
        _client = new ServiceBusClient(configuration.GetValue<string>("AzureServiceBus:ServiceBus"));
        _clientSender = _client.CreateSender(configuration.GetValue<string>("AzureServiceBus:Queue"));
    }

    public async Task SendMessage(Message message) {
        string messagePayload = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(messagePayload);
        await _clientSender.SendMessageAsync(serviceBusMessage);
    }
}
