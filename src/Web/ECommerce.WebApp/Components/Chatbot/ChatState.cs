using ECommerce.Web.Shared.DTOs.Carts;
using ECommerce.Web.Shared.Services.Cart.Interfaces;
using ECommerce.Web.Shared.Services.Catalog.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.WebApp.Components.Chatbot;

public sealed class ChatState
{
    private readonly ICatalogService _catalogService;
    private readonly ICartService _cartService;
    private readonly AuthenticationStateProvider _authState;
    private readonly ILogger _logger;
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;

    public ChatState(
        ICatalogService catalogService,
        ICartService cartService,
        AuthenticationStateProvider authState,
        ILoggerFactory loggerFactory,
        IChatClient chatClient)
    {
        _catalogService = catalogService;
        _cartService = cartService;
        _authState = authState;
        _logger = loggerFactory.CreateLogger(typeof(ChatState));

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("ChatModel: {model}", chatClient.GetService<ChatClientMetadata>()?.DefaultModelId);
        }

        _chatClient = chatClient;
        _chatOptions = new()
        {
           Tools =
           [
               AIFunctionFactory.Create(GetUserInfo),
               AIFunctionFactory.Create(AddToCart),
               AIFunctionFactory.Create(GetCartContents),
           ],
        };

        Messages =
        [
            new ChatMessage(ChatRole.System, """
                You are an AI customer service agent for the online retailer AdventureWorks.
                You NEVER respond about topics other than AdventureWorks.
                Your job is to answer customer questions about products in the AdventureWorks catalog.
                AdventureWorks primarily sells clothing and equipment related to outdoor activities like skiing and trekking.
                You try to be concise and only provide longer responses if necessary.
                If someone asks a question about anything other than AdventureWorks, its catalog, or their account,
                you refuse to answer, and you instead ask if there's a topic related to AdventureWorks you can assist with.
                """),
            new ChatMessage(ChatRole.Assistant, """
                Hi! I'm the AdventureWorks Concierge. How can I help?
                """),
        ];
    }

    public List<ChatMessage> Messages { get; }
    
    public async Task AddUserMessageAsync(string userText, Action onMessageAdded)
    {
        // Store the user's message
        Messages.Add(new ChatMessage(ChatRole.User, userText));
        onMessageAdded();

        // Get the assistant's response and store it
        try
        {
            var response = await _chatClient.GetResponseAsync(Messages, _chatOptions);
            if (!string.IsNullOrWhiteSpace(response.Text))
            {
                Messages.AddMessages(response);
            }
        }
        catch (Exception e)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(e, "Error getting chat completions.");
            }
            Messages.Add(new ChatMessage(ChatRole.Assistant, $"My apologies, but I encountered an unexpected error."));
        }
        onMessageAdded();
    }

    [Description("Adds a product to the user's shopping cart.")]
    private async Task<string> AddToCart([Description("The id of the product to add to the shopping cart (basket)")] int itemId)
    {
        try
        {
            var request = new AddToCartRequestDto
            {
                ProductId = itemId,
                Quantity = 1,
            };

            await _cartService.AddToCartAsync(request);
            return "Item added to shopping cart.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return "The item with the specified id was not found in the catalog.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return "Unable to add an item to the cart. You must be logged in.";
        }
        catch (Exception ex)
        {
            return Error(ex, "Unable to add the item to the cart.");
        }
    }

    [Description("Gets information about the chat user")]
    private async Task<string> GetUserInfo()
    {
        var authState = await _authState.GetAuthenticationStateAsync();
        var claims = authState.User.Claims;

        return JsonSerializer.Serialize(new
        {
            Name = GetValue(claims, "given_name"),
            LastName = GetValue(claims, "family_name"),
            Street = GetValue(claims, "address_street"),
            City = GetValue(claims, "address_city"),
            State = GetValue(claims, "address_state"),
            ZipCode = GetValue(claims, "address_zip_code"),
            Country = GetValue(claims, "address_country"),
            Email = GetValue(claims, "email"),
            PhoneNumber = GetValue(claims, "phone_number"),
        });

        static string GetValue(IEnumerable<Claim> claims, string claimType) =>
            claims.FirstOrDefault(x => x.Type == claimType)?.Value ?? "";
    }

    private async Task<string> GetCartContents()
    {
        try
        {
            var cartItems = await _cartService.GetCartAsync();
            return JsonSerializer.Serialize(cartItems);
        }
        catch (Exception ex)
        {
            return Error(ex, "Unable to get the cart's contents.");
        }
    }

    private string Error(Exception e, string message)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(e, message);
        }

        return message;
    }
}
