using System.Reflection;

namespace ECommerce.Webhooks.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
