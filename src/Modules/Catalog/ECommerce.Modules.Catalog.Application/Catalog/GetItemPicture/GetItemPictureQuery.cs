using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItemPicture;

public sealed record GetItemPictureQuery(int Id) : IQuery<PhysicalFileResponse>;
