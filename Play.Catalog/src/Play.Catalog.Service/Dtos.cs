using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    // for get operations
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);

    // for create operations
    public record CreateItemDto([Required] string Name, string Description, [Range(0, 100)] decimal Price);

    // for update operations
    public record UpdateItemDto([Required] string Name, string Description, [Range(0, 100)] decimal Price);
}