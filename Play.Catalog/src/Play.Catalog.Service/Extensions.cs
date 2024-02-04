using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        // statikuri klasi gvinda rom Items mivabat extension methodad CastDto methodi
        // amis mere Item item = ....; 
        // Item.CastDto() gamodzaxeba shegvedzleba 
        public static ItemDto CastDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }

    }
}