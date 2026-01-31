using Dapper;
using ECommerce.Common.Application.Clock;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Data;
using System.Reflection;

namespace ECommerce.Common.Infrastructure.Inbox;

public abstract class ProcessInboxJobBase(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger<ProcessInboxJobBase> logger) : IJob
{
    protected abstract string ModuleName { get; }
    protected abstract Assembly PresentationAssembly { get; }
    protected abstract string Schema { get; }
    protected abstract int BatchSize { get; }

    public async Task Execute(IJobExecutionContext context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);
        }

        await using var connection = await dbConnectionFactory.OpenConnectionAsync(context.CancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(context.CancellationToken);

        var inboxMessages = await GetUnprocessedInboxMessagesAsync(connection, transaction);

        foreach (var inboxMessage in inboxMessages)
        {
            Exception? exception = null;
            try
            {
                var integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(inboxMessage.Content, SerializerSettings.Instance)!;

                await PublishIntegrationEvent(integrationEvent, context.CancellationToken);
            }
            catch (Exception caughtException)
            {
                logger.LogError(caughtException, "{Module} - Exception while processing inbox message {MessageId}", ModuleName, inboxMessage.Id);

                exception = caughtException;
            }

            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }

        await transaction.CommitAsync(context.CancellationToken);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
        }
    }

    private async Task PublishIntegrationEvent(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var integrationEventHandlers = IntegrationEventHandlersFactory.GetHandlers(
            integrationEvent.GetType(),
            scope.ServiceProvider,
            PresentationAssembly);

        foreach (var integrationEventHandler in integrationEventHandlers)
        {
            await integrationEventHandler.Handle(integrationEvent, cancellationToken);
        }
    }


    private async Task<IReadOnlyList<InboxMessageResponse>> GetUnprocessedInboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                id AS {nameof(InboxMessageResponse.Id)},
                content AS {nameof(InboxMessageResponse.Content)}
             FROM ticketing.inbox_messages
             WHERE processed_at_utc IS NULL
             ORDER BY occurred_at_utc
             LIMIT {BatchSize}
             FOR UPDATE
             """;

        var inboxMessages = await connection.QueryAsync<InboxMessageResponse>(sql, transaction: transaction);

        return inboxMessages.ToList();
    }

    private async Task UpdateInboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {
        string sql =
            $"""
            UPDATE {ModuleName}.inbox_messages
            SET processed_at_utc = @ProcessedAtUtc,
               error = @Error
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                inboxMessage.Id,
                ProcessedAtUtc = dateTimeProvider.UtcNow,
                Error = exception?.Message
            }, transaction: transaction);
    }

    private sealed record InboxMessageResponse(Guid Id, string Content);
}
