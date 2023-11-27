using Azure.Messaging.ServiceBus;
using OrderApi.Shared;
using Serilog;

namespace OrderApi.Services;

public class ReceiverService : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly ProcessService _processService;
    private readonly ServiceBusClient _client;

    public ReceiverService(IConfiguration configuration, ProcessService processService) {
        _configuration = configuration;
        _processService = processService;
        _client = new ServiceBusClient(configuration.GetValue<string>("AzureServiceBus:ServiceBus"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await Register();
    }

    private async Task Register() {
        ServiceBusProcessor processor = _client.CreateProcessor(_configuration.GetValue<string>("AzureServiceBus:Queue"));
        processor.ProcessMessageAsync += ProcessMessagesAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;
        await processor.StartProcessingAsync();
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg) {
        Log.Error("Error during a message processing:" + arg.Exception);
        return Task.CompletedTask;
    }

    private async Task ProcessMessagesAsync(ProcessMessageEventArgs args) {
        var payload = args.Message.Body.ToObjectFromJson<Message>();

        if (payload.Name == "Order") {
            await _processService.Process(payload);
        }
        await args.CompleteMessageAsync(args.Message);
    }
}