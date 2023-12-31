﻿using CardapioOnlineAPI.Dto;
using CardapioOnlineAPI.Entities;
using CardapioOnlineAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardapioOnlineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _service;

        public MenuController(MenuService menuService)
        {
            _service = menuService;
        }

        [HttpGet]
        public IActionResult GetAllMenuItems()
        {
            var resposta = _service.GetAllMenuItems();

            if (resposta == null)
            {
                return new NotFoundResult();
            }

            return Ok(resposta);
        }

        [HttpPost]
        public void AddMenuItem([FromBody] CreateRequest request)
        {
            _service.AddMenuItem(request);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMenuItem(int id, [FromBody] UpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var menuItem = new MenuItem
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Id = request.Id
            };

            _service.UpdateMenuItem(id, menuItem);

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetMenuItemsByID(int id)
        {
            var menuItem = _service.GetMenuItemById(id);

            if(menuItem == null)
            {
                return NotFound();
            }
            return Ok(menuItem); 
        }

        [HttpDelete]
        public IActionResult DeleteMenuItem(int id)
        {
            var menuItem = _service.GetMenuItemById(id);

            if (menuItem == null)
            {
                return NotFound();
            }
            _service.DeleteMenuItem(id);
            return NoContent();

        }

        [HttpPost("{id}/upload/")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var menuItem = _service.GetMenuItemById(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            if(file== null)
            {
                return BadRequest();
            }

            string uploadsFolder = Path.Combine(@"c:\\temp\upload");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using(var fileStrem = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStrem);
            }

            menuItem.ImageUrl = filePath;
            _service.UpdateMenuItem(id, menuItem);

            return NoContent();
        }
        
    }
}
