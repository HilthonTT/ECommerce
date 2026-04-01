namespace ECommerce.Common.Application.DTOs;

public sealed class LinkDto
{
    public required string Href { get; init; }

    public required string Rel { get; init; }

    public required string Method { get; set; }

    public static LinkDto Create(string href, string rel, string method) =>
         new() { Href = href, Rel = rel, Method = method };
}
