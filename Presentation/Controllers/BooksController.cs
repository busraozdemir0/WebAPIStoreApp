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
        public IActionResult GetAllBooks()
        {
            // Global hata yönetimi yaptığımız için try-catch bloklarını kaldırdık. Hata olduğunda o hatayı yakalayıp ilgili kodu ve mesajı bize döndürecektir
            var books = _manager.BookService.GetAllBooks(false); // değişiklikleri izlememesini tercih ettiğimiz için ef core çalışmasında bir performans artışı gözlemleyeceğiz
            return Ok(books);

        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)  // id Route'dan gelecek
        {
            var book = _manager.BookService.GetOneBookById(id, false);
            return Ok(book);

        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] BookDtoForInsertion bookDto)
        {
            if (bookDto is null)
            {
                return BadRequest();  //400 kodu üretecek
            }

            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);  // Bu ifadeyle BookDtoForManipulation'da yazdığımız doğrulama mesajları 422 koduyla birlikte dönüş yapacaktır
            }
            var book = _manager.BookService.CreateOneBook(bookDto);

            return StatusCode(201, book); // 201 kodu => created

        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            if (bookDto is null) // parametre dolu mu boş mu kontrolü
            {
                return BadRequest();  //400 kodu üretecek
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState); // 422 kodu
            }

            _manager.BookService.UpdateOneBook(id, bookDto, true);

            return NoContent(); //204

        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            _manager.BookService.DeleteOneBook(id, false);

            return NoContent();
        }
        [HttpPatch("{id:int}")] // kısmi güncelleme(örneğin bir kaydın sadece başlığını güncellemek gibi)
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest(); // 400 code

            var result = _manager.BookService.GetOneBookForPatch(id, false);

            bookPatch.ApplyTo(result.bookDtoForUpdate ,ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            _manager.BookService.SaveChangesForPatch(result.bookDtoForUpdate, result.book);

            return NoContent(); //204
        }
    }
}
