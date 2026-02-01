using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using ECommerce.Modules.Ticketing.Domain.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using System.Text;
using System.Text.Json;

namespace ECommerce.Modules.Ticketing.Application.Assistant.GetStreamingChatResponse;

internal sealed class GetStreamChatResponseCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IChatClient chatClient,
    IProductRepository productRepository) : ICommandHandler<GetStreamChatResponseCommand>
{
    private const float DefaultTemperature = 0f;
    private const int DefaultSeed = 0;

    public async Task<Result> Handle(GetStreamChatResponseCommand command, CancellationToken cancellationToken)
    {
        Product? product = await GetProductIfSpecifiedAsync(command.ProductId, cancellationToken);

        List<ChatMessage> messages = BuildChatMessages(command, product);

        if (httpContextAccessor.HttpContext?.Response is null)
        {
            return Result.Failure(
                new Error(
                    "HttpResponseNotAvailable",
                    "The HTTP response is not available in the current context.",
                    ErrorType.Problem));
        }

        HttpResponse response = httpContextAccessor.HttpContext?.Response!;

        await WriteResponseStartAsync(response, cancellationToken);

        string answerText = await StreamAssistantResponseAsync(
            messages,
            response,
            command.ProductId,
            cancellationToken);

        await WriteCustomerClassificationIfApplicableAsync(
            response,
            answerText,
            command.CustomerName,
            cancellationToken);

        await WriteResponseEndAsync(response, cancellationToken);

        return Result.Success();
    }

    private async Task<Product?> GetProductIfSpecifiedAsync(Guid? productId, CancellationToken cancellationToken)
    {
        return productId.HasValue
            ? await productRepository.GetAsync(productId.Value, cancellationToken)
            : null;
    }

    private static List<ChatMessage> BuildChatMessages(GetStreamChatResponseCommand command, Product? product)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, BuildSystemPrompt(command, product))
        };

        messages.AddRange(command.Messages.Select(m =>
            new ChatMessage(m.IsAssistant ? ChatRole.Assistant : ChatRole.User, m.Text)));

        return messages;
    }

    private static string BuildSystemPrompt(GetStreamChatResponseCommand command, Product? product)
    {
        return $$"""
            You are a helpful AI assistant called 'Assistant' whose job is to help customer service agents working for AdventureWorks, an online retailer.
            The customer service agent is currently handling the following ticket:

            <product_id>{{command.ProductId}}</product_id>
            <product_name>{{product?.Name ?? "None specified"}}</product_name>
            <customer_name>{{command.CustomerName}}</customer_name>
            <summary>{{command.TicketSummary}}</summary>

            The most recent message from the customer is this:
            <customer_message>{{command.TicketLastCustomerMessage}}</customer_message>
            However, that is only provided for context. You are not answering that question directly. The real question will be asked by the user below.

            If this is a question about the product, ALWAYS search the product manual.

            ALWAYS justify your answer by citing a search result. Do this by including this syntax in your reply:
            <cite searchResultId=number>shortVerbatimQuote</cite>
            shortVerbatimQuote must be a very short, EXACT quote (max 10 words) from whichever search result you are citing.
            Only give one citation per answer. Always give a citation because this is important to the business.
            """;
    }

    private async Task<string> StreamAssistantResponseAsync(
        List<ChatMessage> messages,
        HttpResponse response,
        Guid? productId,
        CancellationToken cancellationToken)
    {
        var searchManual = AIFunctionFactory.Create(
            new SearchManualContext(httpContextAccessor.HttpContext!).SearchManualAsync);

        var chatOptions = new ChatOptions
        {
            Temperature = DefaultTemperature,
            Tools = [searchManual],
            AdditionalProperties = new() { ["seed"] = DefaultSeed },
        };

        var streamingResponse = chatClient.GetStreamingResponseAsync(messages, chatOptions, cancellationToken);

        var answerBuilder = new StringBuilder();

        await foreach (var chunk in streamingResponse)
        {
            var chunkText = chunk.ToString();

            await WriteResponseChunkAsync(response, chunkText, cancellationToken);

            answerBuilder.Append(chunkText);
        }

        return answerBuilder.ToString();
    }

    private async Task WriteCustomerClassificationIfApplicableAsync(
        HttpResponse response,
        string answerText,
        string? customerName,
        CancellationToken cancellationToken)
    {
        var classificationPrompt =
            $"Determine whether the following message is phrased as a reply to the customer {customerName} by name: {answerText}";

        var classification = await chatClient.GetResponseAsync<MessageClassification>(
            classificationPrompt,
            cancellationToken: cancellationToken);

        if (classification.TryGetResult(out var result) && result.IsAddressedToCustomerByName)
        {
            await response.WriteAsync(",\n", cancellationToken);
            await response.WriteAsync(
                JsonSerializer.Serialize(
                    new AssistantChatReplyItem(AssistantChatReplyItemType.IsAddressedToCustomer, "true")),
                cancellationToken);
        }
    }

    private static async Task WriteResponseStartAsync(HttpResponse response, CancellationToken cancellationToken)
    {
        await response.WriteAsync("[null", cancellationToken);
    }

    private static async Task WriteResponseChunkAsync(
        HttpResponse response,
        string chunkText,
        CancellationToken cancellationToken)
    {
        await response.WriteAsync(",\n", cancellationToken);
        await response.WriteAsync(
            JsonSerializer.Serialize(
                new AssistantChatReplyItem(AssistantChatReplyItemType.AnswerChunk, chunkText)),
            cancellationToken);
    }

    private static async Task WriteResponseEndAsync(HttpResponse response, CancellationToken cancellationToken)
    {
        await response.WriteAsync("]", cancellationToken);
    }

    private sealed class MessageClassification
    {
        public bool IsAddressedToCustomerByName { get; set; }
    }
}
