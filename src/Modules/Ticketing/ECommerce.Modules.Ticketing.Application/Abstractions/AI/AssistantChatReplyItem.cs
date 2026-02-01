namespace ECommerce.Modules.Ticketing.Application.Abstractions.AI;

public sealed record AssistantChatReplyItem(
    AssistantChatReplyItemType Type, 
    string Text, 
    int? SearchResultId = null, 
    int? SearchResultProductId = null, 
    int? SearchResultPageNumber = null);
