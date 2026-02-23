using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Assistant.GetStreamingChatResponse;

public sealed record GetStreamChatResponseCommand(
    int? ProductId, 
    string? CustomerName,
    string? TicketSummary, 
    string? TicketLastCustomerMessage,
    IReadOnlyList<GetStreamChatResponseCommand.AssistantChatRequestMessage> Messages) : ICommand
{
    public sealed class AssistantChatRequestMessage
    {
        public bool IsAssistant { get; set; }

        public required string Text { get; set; }
    }
}
