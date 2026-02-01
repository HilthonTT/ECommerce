using ECommerce.Common.Domain;

namespace ECommerce.Common.Application.Sorting;

public interface ISortMappingProvider
{
    SortMapping[] GetMappings<TSource, TDestination>();

    Result ValidateMappings<TSource, TDestination>(string? sort);
}
