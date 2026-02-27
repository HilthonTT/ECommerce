using ECommerce.Common.Application.Clock;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Infrastructure.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Reflection;

namespace ECommerce.Webhooks.Infrastructure.Inbox;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> options,
    ILogger<ProcessInboxJob> logger) : ProcessInboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, logger)
{
    protected override string ModuleName => "Webhooks";

    protected override Assembly PresentationAssembly => Presentation.AssemblyReference.Assembly;

    protected override string Schema => Schemas.Webhooks;

    protected override int BatchSize => options.Value.BatchSize;
}
