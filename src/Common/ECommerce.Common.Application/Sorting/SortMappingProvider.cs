using ECommerce.Common.Domain;

namespace ECommerce.Common.Application.Sorting;

internal sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
    : ISortMappingProvider
{
    public SortMapping[] GetMappings<TSource, TDestination>()
    {
        SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        return sortMappingDefinition is null
            ? throw new InvalidOperationException($"The mapping from '{typeof(TSource).Name}' into '{typeof(TDestination).Name}' is not defined.")
            : sortMappingDefinition.Mappings;
    }

    public Result ValidateMappings<TSource, TDestination>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return Result.Success();
        }

        List<string> sortFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(' ')[0])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        SortMapping[] mapping = GetMappings<TSource, TDestination>();

        bool isValid = sortFields.All(f => mapping.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));

        return isValid
            ? Result.Success()
            : SortErrors.InvalidSortParameter(sort);
    }
}
