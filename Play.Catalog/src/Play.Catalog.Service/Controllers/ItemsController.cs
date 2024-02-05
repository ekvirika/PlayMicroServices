// controller to manage catalog items

/* After adding MassTransit, we define this class as a Producer.
And every Consumer, that is listening to the Queue, will get notified, when 
an item is created, updated or deleted. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{
    /* https://localhost:5001/items  */
    [ApiController] // attribute for api features, request error information
    [Route("items")] // this attribute specifies the url pattern, that this controller will map into
    public class ItemsController : ControllerBase
    {
        /* Dependency Injection */
        private readonly IRepository<Item> itemsRepository;

        /* This class allows us to send messages. */
        private readonly IPublishEndpoint publishEndpoint;
        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.CastDto());
            return Ok(items);
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

            /* We need to send message via RabbitMQ that the new item has been created. */
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

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

            /* We need to send message via RabbitMQ that the new item has been created. */
            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingItem = await itemsRepository.GetOneAsync(id);
            if (existingItem == null) return NotFound();
            await itemsRepository.RemoveAsync(id);

            /* We need to send message via RabbitMQ that the new item has been created. */
            await publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));

            return NoContent();
        }
    }
}