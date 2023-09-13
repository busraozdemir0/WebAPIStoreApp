using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Repositories.EfCore;
using Repositories.Contracts;

namespace WebAPIStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            // DI => Manager sınıfını gördüğü an otomatik olarak new'lemesi için(Program.cs'de yapılandırmayı gerçekleştirdik)
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _manager.Book.GetAllBooks(false);
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)  // id Route'dan gelecek
        {
            try
            {
                var book = _manager.Book.GetOneBookById(id, false);

                if (book is null) // eğer gönderilen id'ye ait kitap yoksa
                {
                    return NotFound();  //404
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                {
                    return BadRequest();  //400 kodu üretecek
                }
                _manager.Book.CreateOneBook(book);
                _manager.Save();  // manager'da kaydetme işlemi metod içerisine yazıldı
                return StatusCode(201, book); // 201 kodu => created

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {
            try
            {
                // check book?
                var entity = _manager.Book.GetOneBookById(id, true); ;

                if (entity is null)
                {
                    return NotFound(); //404
                }

                // check id
                if (id != book.Id)
                    return BadRequest();  //400

                entity.Title = book.Title;
                entity.Price = book.Price;

                _manager.Save();

                return Ok(book);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            try
            {
                var entity = _manager.Book.GetOneBookById(id, false);

                if (entity is null)
                    return NotFound(new // girilen id'ye ait kitap bulunamazsa hata mesajı dönecek 404 koduyla
                    {
                        StatusCode = 404,
                        message = $"Book with id:{id} could not found."
                    });

                _manager.Book.Delete(entity);
                _manager.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        [HttpPatch("{id:int}")] // kısmi güncelleme(örneğin bir kaydın sadece başlığını güncellemek gibi)
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            try
            {
                // kitap var mı kontrolü
                var entity = _manager.Book.GetOneBookById(id, true);
                if (entity is null)
                    return NotFound(); //404

                bookPatch.ApplyTo(entity);
                _manager.Book.Update(entity);
                _manager.Save();

                return NoContent(); //204
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
