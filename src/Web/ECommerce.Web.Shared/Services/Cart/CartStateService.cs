namespace ECommerce.Web.Shared.Services.Cart;

public sealed class CartStateService : IDisposable
{
    private int _itemCount;

    public int ItemCount => _itemCount;

    public event Action? OnChange;

    public void SetItemCount(int count)
    {
        if (_itemCount == count)
        {
            return;
        }

        _itemCount = count;
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        OnChange = null;
    }
}
