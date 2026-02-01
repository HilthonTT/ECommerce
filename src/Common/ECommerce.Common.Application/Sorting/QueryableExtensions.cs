using System.Linq.Dynamic.Core;

namespace ECommerce.Common.Application.Sorting;

public static class QueryableExtensions
{
    private const string DefaultSortDirection = "ASC";
    private const string DescendingSortDirection = "DESC";

    public static IQueryable<T> ApplySort<T>(
      this IQueryable<T> query,
      string? sort,
      SortMapping[] mappings,
      string defaultOrderBy = "Id")
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        string[] sortFields = ParseSortFields(sort);
        if (sortFields.Length == 0)
        {
            return query.OrderBy(defaultOrderBy);
        }

        List<string> orderByParts = BuildOrderByClause(sortFields, mappings);
        if (orderByParts.Count == 0)
        {
            return query.OrderBy(defaultOrderBy);
        }

        return query.OrderBy(string.Join(", ", orderByParts));
    }

    private static string[] ParseSortFields(string sort)
    {
        return sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static List<string> BuildOrderByClause(string[] sortFields, SortMapping[] mappings)
    {
        var orderByParts = new List<string>(sortFields.Length);

        foreach (string field in sortFields)
        {
            (string? sortField, bool isDescending) = ParseSortField(field);
            SortMapping? mapping = FindMapping(mappings, sortField);

            if (mapping is null)
            {
                // Skip invalid fields instead of throwing
                continue;
            }

            string direction = DetermineSortDirection(isDescending, mapping.Reverse);
            orderByParts.Add($"{mapping.PropertyName} {direction}");
        }

        return orderByParts;
    }

    private static (string SortField, bool IsDescending) ParseSortField(string field)
    {
        string[] parts = field.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string sortField = parts[0];
        bool isDescending = parts.Length > 1 &&
                parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortField, isDescending);
    }

    private static SortMapping? FindMapping(SortMapping[] mappings, string sortField)
    {
        return mappings.FirstOrDefault(m => m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));
    }

    private static string DetermineSortDirection(bool isDescending, bool reverse)
    {
        return (isDescending, reverse) switch
        {
            (false, false) => DefaultSortDirection,
            (false, true) => DescendingSortDirection,
            (true, false) => DescendingSortDirection,
            (true, true) => DefaultSortDirection
        };
    }
}
