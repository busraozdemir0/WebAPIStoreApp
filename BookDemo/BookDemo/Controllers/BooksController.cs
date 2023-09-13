using BookDemo.Data;
using BookDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookDemo.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = ApplicationContext.Books;
            return Ok(books);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)  // id Route'dan gelecek
        {
            var book = ApplicationContext.Books.Where(b => b.Id.Equals(id)).SingleOrDefault();

            if (book is null) // eğer gönderilen id'ye ait kitap yoksa
            {
                return NotFound();  //404
            }

            return Ok(book);
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

                ApplicationContext.Books.Add(book);
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
            // check book?
            var entity = ApplicationContext.Books.Find(b => b.Id.Equals(id));

            if (entity is null)
            {
                return NotFound(); //404
            }

            // check id
            if (id != book.Id)
                return BadRequest();  //400

            ApplicationContext.Books.Remove(entity); // girilen id'ye ait kaydı önce kaldırıyoruz
            book.Id = entity.Id;
            ApplicationContext.Books.Add(book); // daha sonra ekliyoruz
            return Ok(book);

        }
        [HttpDelete]
        public IActionResult DeleteAllBooks()
        {
            ApplicationContext.Books.Clear(); // normalde tablonun tümünü silmek mantıksız ama inmemory çalıştığımız için burada örnek olarak yapabiliriz
            return NoContent(); //204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            var entity = ApplicationContext.Books.Find(b => b.Id.Equals(id));

            if (entity is null)
                return NotFound(new // girilen id'ye ait kitap bulunamazsa hata mesajı dönecek 404 koduyla
                {
                    StatusCode = 404,
                    message = $"Book with id:{id} could not found."
                });

            ApplicationContext.Books.Remove(entity);
            return NoContent();
        }

        [HttpPatch("{id:int}")] // kısmi güncelleme(örneğin bir kaydın sadece başlığını güncellemek gibi)
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name ="id")] int id,[FromBody] JsonPatchDocument<Book> bookPatch)
        {
            // kitap var mı kontrolü
            var entity = ApplicationContext.Books.Find(b => b.Id.Equals(id));
            if (entity is null)
                return NotFound(); //404

            bookPatch.ApplyTo(entity);
            return NoContent(); //204
        }
    }
}
