
using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public sealed class CardType : Enumeration<CardType>
{
    public static readonly CardType Visa = new(1, nameof(Visa));
    public static readonly CardType MasterCard = new(2, nameof(MasterCard));
    public static readonly CardType AmericanExpress = new(3, nameof(AmericanExpress));
    public static readonly CardType Discover = new(4, nameof(Discover));

    private CardType(int id, string name)
        : base(id, name)
    {
    }

    private CardType()
    {
    }
}
