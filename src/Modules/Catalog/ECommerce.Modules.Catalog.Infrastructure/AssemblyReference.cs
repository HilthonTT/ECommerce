using System.Reflection;

namespace ECommerce.Modules.Catalog.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
