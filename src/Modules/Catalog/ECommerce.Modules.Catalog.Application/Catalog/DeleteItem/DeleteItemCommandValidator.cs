using FluentValidation;

namespace ECommerce.Modules.Catalog.Application.Catalog.DeleteItem;

internal sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.CatalogItemId)
           .GreaterThan(0);
    }
}
