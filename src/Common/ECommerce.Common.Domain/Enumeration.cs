using System.Reflection;

namespace ECommerce.Common.Domain;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>, IComparable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<Dictionary<int, TEnum>> _enumerations = new(GetAllEnumerationOptions);

    public int Id { get; protected init; }

    public string Name { get; protected init; } = string.Empty;

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    protected Enumeration()
    {
    }

    public static IReadOnlyCollection<TEnum> GetValues()
    {
        return _enumerations.Value.Values.ToList();
    }

    public static TEnum? FromId(int id)
    {
        return _enumerations.Value.TryGetValue(id, out TEnum? enumeration)
            ? enumeration
            : null;
    }

    public static TEnum? FromName(string name)
    {
        return _enumerations.Value.Values.SingleOrDefault(e =>
            e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static bool Contains(int id)
    {
        return _enumerations.Value.ContainsKey(id);
    }

    public static bool Contains(string name)
    {
        return _enumerations.Value.Values.Any(e =>
            e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public bool Equals(Enumeration<TEnum>? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetType() == other.GetType() && other.Id.Equals(Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<TEnum> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(Enumeration<TEnum>? other)
    {
        return other is null ? 1 : Id.CompareTo(other.Id);
    }

    public override string ToString()
    {
        return Name;
    }

    private static Dictionary<int, TEnum> GetAllEnumerationOptions()
    {
        Type enumerationType = typeof(TEnum);

        return enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(null)!)
            .ToDictionary(x => x.Id);
    }

    public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right)
    {
        return !(left == right);
    }
}
