using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogId, int Quantity, DateTimeOffset AcquairedDate);
}