using ECommerce.Common.Application.Clock;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Reflection;

namespace ECommerce.Modules.Catalog.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger)
    : ProcessOutboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, logger)
{
    protected override string ModuleName => "Catalog";
    protected override Assembly ApplicationAssembly => Application.AssemblyReference.Assembly;
    protected override string Schema => Schemas.Catalog;
    protected override int BatchSize => outboxOptions.Value.BatchSize;
}