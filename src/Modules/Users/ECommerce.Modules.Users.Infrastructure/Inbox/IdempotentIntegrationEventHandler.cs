using Dapper;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Infrastructure.Inbox;
using System.Data;

namespace ECommerce.Modules.Users.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory dbConnectionFactory)
    : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public override async Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxConsumerExistsAsync(
        IDbConnection connection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            $"""
            SELECT EXISTS(
                SELECT 1
                FROM {Schemas.Users}.inbox_message_consumers
                WHERE inbox_message_id = @InboxMessageId AND 
                      name = @Name
            )
            """;

        return await connection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private static async Task InsertInboxConsumerAsync(
        IDbConnection connection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            $"""
            INSERT INTO {Schemas.Users}.inbox_message_consumers(inbox_message_id, name)
            VALUES (@InboxMessageId, @Name)
            """;

        await connection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}