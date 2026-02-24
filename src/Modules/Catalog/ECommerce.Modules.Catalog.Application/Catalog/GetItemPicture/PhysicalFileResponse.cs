namespace ECommerce.Modules.Catalog.Application.Catalog.GetItemPicture;

public sealed record PhysicalFileResponse(string Path, string MimeType, DateTimeOffset? LastModified);
