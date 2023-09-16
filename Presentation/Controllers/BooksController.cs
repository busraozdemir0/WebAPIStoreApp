using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
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
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            // Global hata yönetimi yaptığımız için try-catch bloklarını kaldırdık. Hata olduğunda o hatayı yakalayıp ilgili kodu ve mesajı bize döndürecektir
            var books = await _manager.BookService.GetAllBooksAsync(false); // değişiklikleri izlememesini tercih ettiğimiz için ef core çalışmasında bir performans artışı gözlemleyeceğiz
            return Ok(books);

        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)  // id Route'dan gelecek
        {
            var book =await _manager.BookService.GetOneBookByIdAsync(id, false);
            return Ok(book);

        }
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            if (bookDto is null)
            {
                return BadRequest();  //400 kodu üretecek
            }

            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);  // Bu ifadeyle BookDtoForManipulation'da yazdığımız doğrulama mesajları 422 koduyla birlikte dönüş yapacaktır
            }
            var book = await _manager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book); // 201 kodu => created

        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            if (bookDto is null) // parametre dolu mu boş mu kontrolü
            {
                return BadRequest();  //400 kodu üretecek
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState); // 422 kodu
            }

            await _manager.BookService.UpdateOneBookAsync(id, bookDto, true);

            return NoContent(); //204

        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
           await _manager.BookService.DeleteOneBookAsync(id, false);

            return NoContent();
        }
        [HttpPatch("{id:int}")] // kısmi güncelleme(örneğin bir kaydın sadece başlığını güncellemek gibi)
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest(); // 400 code

            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

            bookPatch.ApplyTo(result.bookDtoForUpdate ,ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);

            return NoContent(); //204
        }
    }
}
