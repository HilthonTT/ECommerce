using ECommerce.Common.Application.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Memory;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ECommerce.Modules.Ticketing.Application.Abstractions.AI;

internal sealed class SearchManualContext(HttpContext httpContext)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IProductManualSemanticSearch _manualSearch = httpContext
        .RequestServices.GetRequiredService<IProductManualSemanticSearch>();

    public async Task<object> SearchManualAsync(
        [Description("A phrase to use when searching the manual")] string searchPhrase,
        [Description("ID for the product whose manual to search. Set to null only if you must search across all product manuals.")] Guid? productId)
    {
        await _semaphore.WaitAsync();

        try
        {
            // Notify the UI we're doing a search
            await httpContext.Response.WriteAsync(",\n");
            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(new AssistantChatReplyItem(AssistantChatReplyItemType.Search, searchPhrase)));

            // Do the search, and supply the results to the UI so it can show one as a citation link
            var searchResults = await _manualSearch.SearchAsync(productId, searchPhrase);
            foreach (var r in searchResults)
            {
                await httpContext.Response.WriteAsync(",\n");
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new AssistantChatReplyItem(
                    AssistantChatReplyItemType.SearchResult,
                    string.Empty,
                    int.Parse(r.Metadata.Id),
                    GetProductId(r),
                    GetPageNumber(r))));
            }

            // Return the search results to the assistant
            return searchResults.Select(r => new
            {
                ProductId = GetProductId(r),
                SearchResultId = r.Metadata.Id,
                r.Metadata.Text,
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static int? GetProductId(MemoryQueryResult result)
    {
        var match = Regex.Match(result.Metadata.ExternalSourceName, @"productid:(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : null;
    }

    private static int? GetPageNumber(MemoryQueryResult result)
    {
        var match = Regex.Match(result.Metadata.AdditionalMetadata, @"pagenumber:(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : null;
    }
}
