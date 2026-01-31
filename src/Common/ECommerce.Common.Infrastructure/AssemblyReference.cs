using System.Reflection;

namespace ECommerce.Common.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
