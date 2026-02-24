using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItemPicture;

internal sealed class GetItemPictureQueryHandler(IDbContext dbContext, IWebHostEnvironment environment)
    : IQueryHandler<GetItemPictureQuery, PhysicalFileResponse>
{
    public async Task<Result<PhysicalFileResponse>> Handle(GetItemPictureQuery query, CancellationToken cancellationToken)
    {
        CatalogItem? catalogItem = await dbContext.CatalogItems
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (catalogItem is null || catalogItem.PictureFileName is null)
        {
            return CatalogItemErrors.NotFound(query.Id);
        }

        string path = GetFullPath(environment.ContentRootPath, catalogItem.PictureFileName);

        string imageFileExtension = Path.GetExtension(catalogItem.PictureFileName);
        string mimeType = GetImageMimeTypeFromImageFileExtension(imageFileExtension);
        DateTimeOffset lastModified = File.GetLastWriteTimeUtc(path);

        return new PhysicalFileResponse(path, mimeType, lastModified);
    }

    private static string GetImageMimeTypeFromImageFileExtension(string extension) => extension switch
    {
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".bmp" => "image/bmp",
        ".tiff" => "image/tiff",
        ".wmf" => "image/wmf",
        ".jp2" => "image/jp2",
        ".svg" => "image/svg+xml",
        ".webp" => "image/webp",
        _ => "application/octet-stream",
    };

    private static string GetFullPath(string contentRootPath, string pictureFileName) =>
       Path.Combine(contentRootPath, "Pics", pictureFileName);
}
