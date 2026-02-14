using System.Diagnostics;

namespace ECommerce.Webhooks.Infrastructure.OpenTelemetry;

internal static class DiagnosticConfig
{
    internal static readonly ActivitySource Source = new("webhooks.infrastructure");
}
