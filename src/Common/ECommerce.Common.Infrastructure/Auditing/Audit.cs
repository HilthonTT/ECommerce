using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommerce.Common.Infrastructure.Auditing;

public sealed class Audit
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string TableName { get; init; } = string.Empty;
    public DateTime OccurredAtUtc { get; init; }
    public string PrimaryKey { get; init; } = string.Empty;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? AffectedColumns { get; init; }

    private Audit() { }

    public static Audit Create(
        string userId,
        string auditType,
        string tableName,
        DateTime occurredAtUtc,
        string primaryKey,
        string? oldValues,
        string? newValues,
        string? affectedColumns)
    {
        var audit = new Audit
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Type = auditType,
            TableName = tableName,
            OccurredAtUtc = occurredAtUtc,
            PrimaryKey = primaryKey,
            OldValues = oldValues,
            NewValues = newValues,
            AffectedColumns = affectedColumns
        };

        return audit;
    }
}

public sealed class WriteAuditLogInterceptor(IAuditingUserProvider auditingUserProvider)
    : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            await WriteAuditLog(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task WriteAuditLog(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var audits = CreateAudits(context);

        await context.AddRangeAsync(audits, cancellationToken);
    }

    private IEnumerable<Audit> CreateAudits(DbContext context)
    {
        var userId = auditingUserProvider.GetUserId();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            if (!entry.ShouldBeAudited()) continue;

            var tableName = context
                .Model
                .FindEntityType(entry.Entity.GetType())
                ?.GetTableName() ?? "Unknown Table";

            var auditEntry = new AuditEntry(
                entry,
                tableName,
                userId);

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                if (!property.IsAuditable())
                {
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        auditEntry.AuditType = AuditType.None;
                        break;
                    default:
                        throw new ECommerceException(
                            nameof(WriteAuditLog),
                            Error.Failure(
                                "AuditLog.Failure",
                                "Unable to determine entity state for audit log."));
                }
            }
        }

        return auditEntries.Select(auditEntry => auditEntry.ToAudit());
    }
}
