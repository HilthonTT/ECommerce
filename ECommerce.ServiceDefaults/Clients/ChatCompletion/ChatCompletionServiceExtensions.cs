using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;

namespace ECommerce.ServiceDefaults.Clients.ChatCompletion;

public static class ChatCompletionServiceExtensions
{
    public static void AddChatCompletionService(this IHostApplicationBuilder builder, string serviceName)
    {
        ChatClientBuilder chatClientBuilder = (builder.Configuration[$"{serviceName}:Type"] == "ollama") ?
            builder.AddOllamaChatClient(serviceName) :
            builder.AddOpenAIChatClient(serviceName);

        chatClientBuilder
            .UseFunctionInvocation()
            .UseCachingForTest()
            .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true);
    }
}
