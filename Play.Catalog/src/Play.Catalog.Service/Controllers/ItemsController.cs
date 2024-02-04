// controller to manage catalog items

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    /* https://localhost:5001/items  */
    [ApiController] // attribute for api features, request error information
    [Route("items")] // this attribute specifies the url pattern, that this controller will map into
    public class ItemsController : ControllerBase
    {
        /* Dependency Injection */
        private readonly IRepository<Item> itemsRepository;
        public ItemsController(IRepository<Item> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.CastDto());
            return items;
        }

        /*  GET /items/{id} */
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetOneAsync(id);
            if (item == null) return NotFound();
            return item.CastDto();
        }


        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.Now,
                Id = Guid.NewGuid()
            };
            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetOneAsync(id);
            if (existingItem == null) return NotFound();
            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;
            await itemsRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await itemsRepository.GetOneAsync(id);
            if (existingItem == null) return NotFound();
            await itemsRepository.RemoveAsync(id);
            return NoContent();
        }
    }
}