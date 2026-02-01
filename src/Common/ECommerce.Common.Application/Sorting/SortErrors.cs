using ECommerce.Common.Domain;

namespace ECommerce.Common.Application.Sorting;

public static class SortErrors
{
    public static Error InvalidSortParameter(string sort) => Error.Problem(
        "Sort.InvalidSortParameter",
        $"The provided sort parameter isn't valid: '{sort}'");
}
