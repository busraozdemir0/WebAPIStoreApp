using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController:ControllerBase
    {
        private readonly IServiceManager _services;

        public CategoriesController(IServiceManager services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            return Ok(await _services.CategoryService.GetAllCategoriesAsync(false));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAllCategoryByIdAsync([FromRoute] int id)
        {
            return Ok(await _services.CategoryService.GetOneCategoryByIdAsync(id, false));
        }
        [HttpPost(Name ="create")]
        public async Task<IActionResult> CreateOneCategoryAsync([FromBody] CategoryDtoForInsertion categoryDto)
        {
            var category=await _services.CategoryService.CreateOneCategoryAsync(categoryDto);
            return StatusCode(201, category);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneCategoryAsync([FromRoute(Name ="id")] int id)
        {
            await _services.CategoryService.DeleteOneCategoryAsync(id, false);
            return NoContent();
        }
    }
}
