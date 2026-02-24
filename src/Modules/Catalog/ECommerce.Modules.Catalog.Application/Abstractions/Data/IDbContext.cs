using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Catalog.Application.Abstractions.Data;

public interface IDbContext
{
    DbSet<CatalogItem> CatalogItems { get; }
}
