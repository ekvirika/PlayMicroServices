using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;

        public ItemsController(IRepository<InventoryItem> repository)
        {
            this.itemsRepository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAllAsync(Guid UserId)
        {
            if (UserId == Guid.Empty) return BadRequest();
            var items = (await itemsRepository.GetAllAsync(item => item.UserId == UserId))
                        .Select(item => item.CastDto());
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
        {
            /* Two cases */
            var inventoryItem = await itemsRepository.GetOneAsync(item =>
                                item.UserId == grantItemDto.UserId && item.CatalogItemId == grantItemDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem()
                {
                    CatalogItemId = grantItemDto.CatalogItemId,
                    UserId = grantItemDto.UserId,
                    AcquairedDate = DateTimeOffset.UtcNow,
                    Quantity = grantItemDto.Quantity
                };
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }

    }
}