using Dapper;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Common.Infrastructure.Database;
using ECommerce.Common.Infrastructure.Outbox;
using System.Data;

namespace ECommerce.Modules.Catalog.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync();

        var outboxMessageConsumer = new OutboxMessageConsumer(domainEvent.Id, decorated.GetType().Name);

        if (await OutboxConsumerExistsAsync(connection, outboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(domainEvent, cancellationToken);

        await InsertOutboxConsumerAsync(connection, outboxMessageConsumer);
    }

    private static async Task<bool> OutboxConsumerExistsAsync(
        IDbConnection connection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            $"""
            SELECT EXISTS(
                SELECT 1
                FROM {Schemas.Catalog}.outbox_message_consumers
                WHERE outbox_message_id = @OutboxMessageId AND 
                      name = @Name
            )
            """;

        return await connection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer);
    }

    private static async Task InsertOutboxConsumerAsync(
        IDbConnection connection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            $"""
            INSERT INTO {Schemas.Catalog}.outbox_message_consumers(outbox_message_id, name)
            VALUES (@OutboxMessageId, @Name)
            """;

        await connection.ExecuteAsync(sql, outboxMessageConsumer);
    }
}
