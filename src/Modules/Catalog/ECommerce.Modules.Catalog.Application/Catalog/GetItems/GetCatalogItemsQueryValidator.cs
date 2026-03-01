using FluentValidation;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems;

internal sealed class GetCatalogItemsQueryValidator : AbstractValidator<GetCatalogItemsQuery>
{
    public GetCatalogItemsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
